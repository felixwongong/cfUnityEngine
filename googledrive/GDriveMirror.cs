#if CF_GOOGLE_DRIVE
using System;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Extension;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [InitializeOnLoad]
    public class GDriveMirror
    {
        private static GDriveMirror _instance;
        
        static GDriveMirror()
        {
            Debug.Log("static Init");
            _instance = new GDriveMirror();
        }

        public DateTime lastRefreshTime { get; private set; } = DateTime.MinValue;
        private CancellationTokenSource _refreshCancelToken;

        private GDriveMirror()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        ~GDriveMirror()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            var refreshInterval = GDriveMirrorSetting.GetSetting().refreshIntervalSecond;
            if ((DateTime.Now - lastRefreshTime).TotalSeconds < refreshInterval)
            {
                return;
            }

            if (_refreshCancelToken != null)
            {
                _refreshCancelToken.Cancel();
            }
            
            RefreshAsync().ContinueWithSynchronized(result =>
            {
                if (result.IsFaulted)
                {
                    Debug.LogException(result.Exception);
                }
            });
            
            lastRefreshTime = DateTime.Now;
        }

        private async Task RefreshAsync()
        {
            _refreshCancelToken = new CancellationTokenSource();
            
            var setting = GDriveMirrorSetting.GetSetting();
            var credentialJson = setting.serviceAccountCredentialJson;
            if (credentialJson == null) return;
            
            Debug.Log(credentialJson?.text);
            var credential = GoogleCredential.FromJson(credentialJson.text)
                .CreateScoped(new[] { DriveService.ScopeConstants.DriveReadonly });

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            var request = service.Files.List();
            var response = await request.ExecuteAsync(_refreshCancelToken.Token);
            foreach (var file in response.Files)
            {
                Debug.Log($"{file.Name} ({file.Id}), {file.Kind}, {file.ModifiedTimeDateTimeOffset}");
            }

            _refreshCancelToken = null;
        }
    }
}
#endif
