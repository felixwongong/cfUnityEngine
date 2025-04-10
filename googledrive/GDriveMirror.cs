#if CF_GOOGLE_DRIVE
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        Task RefreshFiles(FilesResource filesResource, IEnumerable<File> googleFiles);
    }


    [InitializeOnLoad]
    public class GDriveMirror
    {
        public static GDriveMirror instance { get; }

        static GDriveMirror()
        {
            instance = new GDriveMirror(new AssetDirectFileMirror());
        }

        public DateTime lastRefreshTime { get; private set; } = DateTime.MinValue;
        private CancellationTokenSource _refreshCancelToken;
        private readonly IFileMirrorHandler _mirrorHandler;

        public GDriveMirror(IFileMirrorHandler mirrorHandler)
        {
            _mirrorHandler = mirrorHandler;
        }

        public async Task RefreshAsync()
        {
            _refreshCancelToken = new CancellationTokenSource();
            
            var setting = GDriveMirrorSetting.GetSetting();
            var credentialJson = setting.serviceAccountCredentialJson;
            if (credentialJson == null) return;
            
            var credential = GoogleCredential.FromJson(credentialJson.text)
                .CreateScoped(DriveService.ScopeConstants.Drive, DriveService.ScopeConstants.DriveMetadata);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            var request = service.Files.List();
            request.Fields = "files(id, name, mimeType, createdTime, modifiedTime, size, owners)";
            var response = await request.ExecuteAsync(_refreshCancelToken.Token);
            await _mirrorHandler.RefreshFiles(service.Files, response.Files);
            
            Debug.Log("[GDriveMirror.RefreshAsync] refresh files succeed");

            _refreshCancelToken = null;
            
            lastRefreshTime = DateTime.Now;
        }
    }
}
#endif
