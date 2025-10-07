#if CF_GOOGLE_DRIVE
using System;
using System.Collections.Generic;
using System.Threading;
using cfEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using ILogger = cfEngine.Logging.ILogger;

namespace cfUnityEngine.GoogleDrive
{
    public struct RefreshStatus
    {
        public readonly File file;
        public readonly IDownloadProgress status;
        public readonly float progress;

        public RefreshStatus(File file, IDownloadProgress status, float progress)
        {
            this.file = file;
            this.status = status;
            this.progress = progress;
        }
    }

    public struct RefreshRequest
    {
        public IList<GoogleFile> googleFiles;
        public Func<GoogleFile, Res<Optional<SettingItem>, Exception>> getSetting;
        public IChangeHandler changeHandler;
    }

    public struct RefreshResult
    {
        public string newChangeChecksumToken;
    }
    
    public interface IFileMirrorHandler
    {
        IAsyncEnumerable<RefreshStatus> RefreshFilesAsync(DriveService driveService, RefreshRequest request);
        void RefreshFiles(DriveService driveService, in RefreshRequest request);
    }


    public partial class GDriveMirror
    {
        private CancellationTokenSource _refreshCancelToken;
        private readonly IFileMirrorHandler _mirrorHandler;
        private readonly ILogger logger;

        public GDriveMirror(IFileMirrorHandler mirrorHandler, ILogger logger)
        {
            _mirrorHandler = mirrorHandler;
            this.logger = logger;
        }

        private DriveService CreateDriveService()
        {
            var setting = GDriveMirrorSetting.GetSetting();
            var credentialJson = setting.serviceAccountCredentialJson;
            if (string.IsNullOrEmpty(credentialJson))
            {
                logger.LogInfo("[GDriveMirror.CreateFileRequest] setting.serviceAccountCredentialJson is null, refresh failed");
                return null;
            }

            var credential = GoogleCredential.FromJson(credentialJson)
                .CreateScoped(DriveService.ScopeConstants.Drive, DriveService.ScopeConstants.DriveMetadata);

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }
        
        private FilesResource.ListRequest CreateFileRequest(DriveService service)
        {
            const string FIELDS = "files(id, name, mimeType, modifiedTime, md5Checksum, size)"; 
            var request = service.Files.List();
            request.Fields = FIELDS;
            return request;
        }

        public IAsyncEnumerable<RefreshStatus> ClearAllAndRefreshAsync()
        {
            var setting = GDriveMirrorSetting.GetSetting();
            setting.changeChecksumToken = string.Empty;
            foreach (var item in setting.settingMap.Values)
            {
                item.googleFileName = string.Empty;
            }
            return RefreshAsync();
        }
        
        public async IAsyncEnumerable<RefreshStatus> RefreshAsync()
        {
            logger.LogInfo("[GDriveMirror.RefreshAsync] start refresh files");

            _refreshCancelToken = new CancellationTokenSource();
            
            var driveService = CreateDriveService();
            if(driveService == null) yield break;

            var request = CreateFileRequest(driveService);
            if(request == null) yield break;

            var changeHandler = new ChangeHandler(GoogleDriveUtil.unityLogger);
            var newChangeChecksumToken = await changeHandler.LoadChangesAsync(driveService, GDriveMirrorSetting.GetSetting().changeChecksumToken);
            var response = await request.ExecuteAsync(_refreshCancelToken.Token);
            var refreshRequest = new RefreshRequest()
            {
                googleFiles = response.Files,
                getSetting = GetSetting,
                changeHandler = changeHandler
            };

            await foreach (var status in _mirrorHandler.RefreshFilesAsync(driveService, refreshRequest))
            {
                yield return status;
            }
            
            GDriveMirrorSetting.GetSetting().changeChecksumToken = newChangeChecksumToken;
            logger.LogInfo("[GDriveMirror.RefreshAsync] refresh files succeed");

            _refreshCancelToken = null;
        }

        public void Refresh()
        {
            logger.LogInfo("[GDriveMirror.Refresh] start refresh files");
            
            var driveService = CreateDriveService();
            if(driveService == null) return;

            var request = CreateFileRequest(driveService);
            if(request == null) return;

            var changeHandler = new ChangeHandler(GoogleDriveUtil.unityLogger);
            var newChecksumToken = changeHandler.LoadChanges(driveService, GDriveMirrorSetting.GetSetting().changeChecksumToken);
            var response = request.Execute();
            var refreshRequest = new RefreshRequest()
            {
                googleFiles = response.Files,
                getSetting = GetSetting,
                changeHandler = changeHandler 
            };

            try
            {
                _mirrorHandler.RefreshFiles(driveService, in refreshRequest);
            }
            catch (Exception e)
            {
                logger.LogException(new Exception("[GDriveMirror.Refresh] refresh files failed", e));
                return;
            }
            
            GDriveMirrorSetting.GetSetting().changeChecksumToken = newChecksumToken;
            logger.LogInfo("[GDriveMirror.Refresh] refresh files succeed");
        }

        private Res<Optional<SettingItem>, Exception> GetSetting(File file)
        {
            var filesSetting = GDriveMirrorSetting.GetSetting(); 
            if(filesSetting == null || filesSetting.settingMap == null)
                return Res.Err<Optional<SettingItem>>(new Exception("GDriveMirrorSetting is not initialized."));

            if (!filesSetting.settingMap.TryGetValue(file.Id, out var setting))
            {
                return Res.Ok(Optional.None<SettingItem>());
            }
            
            return Res.Ok(Optional.Some(setting));
        }
    }
}
#endif
