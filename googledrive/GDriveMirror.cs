#if CF_GOOGLE_DRIVE
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfEngine.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;

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
        IAsyncEnumerable<RefreshStatus> RefreshFilesAsync(FilesResource filesResource, IReadOnlyList<File> googleFiles);
        void RefreshFiles(FilesResource filesResource, IEnumerable<File> googleFiles);
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

        private DriveService CreateFileService()
        {
            var setting = GDriveMirrorSetting.GetSetting();
            var credentialJson = setting.serviceAccountCredentialJson;
            if (credentialJson == null)
            {
                _logger.LogInfo("[GDriveMirror.CreateFileRequest] setting.serviceAccountCredentialJson is null, refresh failed");
                return null;
            }

            var credential = GoogleCredential.FromJson(credentialJson.text)
                .CreateScoped(DriveService.ScopeConstants.Drive, DriveService.ScopeConstants.DriveMetadata);

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        private FilesResource.ListRequest GetServiceRequests(DriveService service)
        {
            var request = service.Files.List();
            request.Fields = "files(id, name, mimeType, createdTime, modifiedTime, size, owners)";
            return request;
        }  

        public async IAsyncEnumerable<RefreshStatus> RefreshAsync()
        {
            _logger.LogInfo("[GDriveMirror.RefreshAsync] start refresh files");

            _refreshCancelToken = new CancellationTokenSource();
            
            var service = CreateFileService();
            if(service == null) yield break;

            var request = GetServiceRequests(service);
            if(request == null) yield break;
            
            var response = await request.ExecuteAsync(_refreshCancelToken.Token);
            
            await foreach (var status in _mirrorHandler.RefreshFilesAsync(service.Files, response.Files.ToArray()))
            {
                yield return status;
            }
            
            _logger.LogInfo("[GDriveMirror.RefreshAsync] refresh files succeed");

            _refreshCancelToken = null;
        }

        public void Refresh()
        {
            _logger.LogInfo("[GDriveMirror.Refresh] start refresh files");
            
            var service = CreateFileService();
            if(service == null) return;

            var request = GetServiceRequests(service);
            if(request == null) return;

            var response = request.Execute();
            _mirrorHandler.RefreshFiles(service.Files, response.Files);
            _logger.LogInfo("[GDriveMirror.Refresh] refresh files succeed");
        }
    }
}
#endif
