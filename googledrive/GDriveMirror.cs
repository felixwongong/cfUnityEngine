#if CF_GOOGLE_DRIVE
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using cfEngine;
using cfEngine.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

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
    
    public interface IFileMirrorHandler
    {
        IAsyncEnumerable<RefreshStatus> RefreshFilesAsync(DriveService driveService, IReadOnlyList<GoogleFile> googleFiles, [NotNull] Func<GoogleFile, Res<Optional<SettingItem>, Exception>> getSetting);
        void RefreshFiles(FilesResource filesResource, IEnumerable<GoogleFile> googleFiles, [NotNull] Func<GoogleFile, Res<Optional<SettingItem>, Exception>> getSetting);
        void ClearAssetDirectories(IEnumerable<string> assetFolderPaths);
    }


    public partial class GDriveMirror
    {
        private CancellationTokenSource _refreshCancelToken;
        private readonly IFileMirrorHandler _mirrorHandler;
        private readonly ILogger _logger;

        public GDriveMirror(IFileMirrorHandler mirrorHandler, ILogger logger)
        {
            _mirrorHandler = mirrorHandler;
            _logger = logger;
        }

        private DriveService CreateDriveService()
        {
            var setting = GDriveMirrorSetting.GetSetting();
            var credentialJson = setting.serviceAccountCredentialJson;
            if (string.IsNullOrEmpty(credentialJson))
            {
                _logger.LogInfo("[GDriveMirror.CreateFileRequest] setting.serviceAccountCredentialJson is null, refresh failed");
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
            const string FIELDS = "files(id, name, mimeType, modifiedTime, size)"; 
            var request = service.Files.List();
            request.Fields = FIELDS;
            return request;
        }

        public IAsyncEnumerable<RefreshStatus> ClearAllAndRefreshAsync()
        {
            var setting = GDriveMirrorSetting.GetSetting();
            var folderPaths = setting.mirrorMap.Values.Select(mirrorItem => mirrorItem.assetFolderPath).ToArray();
            _mirrorHandler.ClearAssetDirectories(folderPaths);
            _logger.LogInfo($"[GDriveMirror.ClearAllAndRefreshAsync] cleared all asset directories, Folders: {string.Join(',', folderPaths)}");
            return RefreshAsync();
        }
        
        public async IAsyncEnumerable<RefreshStatus> RefreshAsync()
        {
            _logger.LogInfo("[GDriveMirror.RefreshAsync] start refresh files");

            _refreshCancelToken = new CancellationTokenSource();
            
            var driveService = CreateDriveService();
            if(driveService == null) yield break;

            var request = CreateFileRequest(driveService);
            if(request == null) yield break;
            
            var response = await request.ExecuteAsync(_refreshCancelToken.Token);

            await foreach (var status in _mirrorHandler.RefreshFilesAsync(driveService, response.Files.ToArray(), GetSetting))
            {
                yield return status;
            }
            
            _logger.LogInfo("[GDriveMirror.RefreshAsync] refresh files succeed");

            _refreshCancelToken = null;
        }

        public void Refresh()
        {
            _logger.LogInfo("[GDriveMirror.Refresh] start refresh files");
            
            var service = CreateDriveService();
            if(service == null) return;

            var request = CreateFileRequest(service);
            if(request == null) return;

            var response = request.Execute();
            
            _mirrorHandler.RefreshFiles(service.Files, response.Files, GetSetting);
            _logger.LogInfo("[GDriveMirror.Refresh] refresh files succeed");
        }

        private Res<Optional<SettingItem>, Exception> GetSetting(File file)
        {
            var filesSetting = GDriveMirrorSetting.GetSetting(); 
            if(filesSetting == null || filesSetting.mirrorMap == null)
                return Res.Err<Optional<SettingItem>>(new Exception("GDriveMirrorSetting is not initialized."));

            if (!filesSetting.mirrorMap.TryGetValue(file.Id, out var setting))
            {
                return Res.Ok(Optional.None<SettingItem>());
            }
            
            return Res.Ok(Optional.Some(setting));
        }
    }
}
#endif
