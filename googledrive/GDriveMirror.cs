#if CF_GOOGLE_DRIVE
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Extension;
using cfUnityEngine.Editor;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    public interface IFileMirrorHandler
    {
        Task RefreshFilesAsync(FilesResource filesResource, IEnumerable<File> googleFiles);
        void RefreshFiles(FilesResource filesResource, IEnumerable<File> googleFiles);
    }


    [InitializeOnLoad]
    public class GDriveMirror
    {
        public static GDriveMirror instance { get; }

        static GDriveMirror()
        {
            instance = new GDriveMirror(new AssetDirectFileMirror());
            
            EditorPlayIntercept.instance.RegisterPlayModeIntercept(() =>
            {
                EditorUtility.DisplayProgressBar("GDriveMirror", "Refreshing files...", 0.5f);
                try
                {
                    instance.RefreshAsync().Wait();
                }
                catch (Exception e)
                {
                    Debug.Log("[GDriveMirror] Refresh failed");
                }
                
                EditorUtility.ClearProgressBar();
            });
        }

        private CancellationTokenSource _refreshCancelToken;
        private readonly IFileMirrorHandler _mirrorHandler;

        public GDriveMirror(IFileMirrorHandler mirrorHandler)
        {
            _mirrorHandler = mirrorHandler;
        }

        private DriveService CreateFileService()
        {
            _refreshCancelToken = new CancellationTokenSource();
            
            var setting = GDriveMirrorSetting.GetSetting();
            var credentialJson = setting.serviceAccountCredentialJson;
            if (credentialJson == null)
            {
                Debug.Log("[GDriveMirror.CreateFileRequest] setting.serviceAccountCredentialJson is null, refresh failed");
                return null;
            }

            var credential = GoogleCredential.FromJson(credentialJson.text)
                .CreateScoped(DriveService.ScopeConstants.Drive, DriveService.ScopeConstants.DriveMetadata);

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        private FilesResource.ListRequest getServiceRequests(DriveService service)
        {
            var request = service.Files.List();
            request.Fields = "files(id, name, mimeType, createdTime, modifiedTime, size, owners)";
            return request;
        }  

        public async Task RefreshAsync()
        {
            Debug.Log("[GDriveMirror.RefreshAsync] start refresh files");

            var service = CreateFileService();
            if(service == null) return;

            var request = getServiceRequests(service);
            if(request == null) return;
            
            var response = await request.ExecuteAsync(_refreshCancelToken.Token);
            await _mirrorHandler.RefreshFilesAsync(service.Files, response.Files);
            
            Debug.Log("[GDriveMirror.RefreshAsync] refresh files succeed");

            _refreshCancelToken = null;
        }

        public void Refresh()
        {
            Debug.Log("[GDriveMirror.Refresh] start refresh files");
            
            var service = CreateFileService();
            if(service == null) return;

            var request = getServiceRequests(service);
            if(request == null) return;

            var response = request.Execute();
        }
    }
}
#endif
