using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cfUnityEngine.Util;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    public struct FolderMimeHandler : FileHandler
    {
        private class DownloadProgress : IDownloadProgress
        {
            private DownloadStatus _status;
            private long _byteDownloaded;
            private Exception _exception;

            public DownloadStatus Status { get { Commit(); return _status; } } 
            public long BytesDownloaded { get { Commit(); return _byteDownloaded; } }
            public Exception Exception { get { Commit(); return _exception; } }
            
            private List<IDownloadProgress> _progressList = new();
            private bool isCommitted = false;

            public void AddProgress(IDownloadProgress progress)
            {
                _progressList.Add(progress);
            }

            private void Commit()
            {
                if (isCommitted)
                    return;

                _status = _progressList.All(x => x.Status == DownloadStatus.Completed) ? DownloadStatus.Completed : DownloadStatus.Failed;
                _byteDownloaded = _progressList.Sum(x => x.BytesDownloaded);
                if (_progressList.Any(x => x.Exception != null))
                {
                    _exception = new AggregateException(_progressList.Where(x => x.Exception != null).Select(x => x.Exception));
                }
                else
                {
                    _exception = null;
                }
                isCommitted = true;
            }
        }
        
        public IDownloadProgress DownloadWithStatus(FilesResource filesResource, in FileHandler.DownloadRequest downloadRequest)
        {
            var googleFileId = downloadRequest.googleFileId;
            var subItemRequest = CreateGoogleFolderItemRequest(filesResource, googleFileId);
            var result =  subItemRequest.Execute();
            var folderItems = result.Files;
            var directory = new DirectoryInfo(Path.Combine(downloadRequest.rootDirectoryInfo.FullName, downloadRequest.localName));
            if (directory.Exists)
            {
                directory.Delete(true);
                Debug.Log($"[FolderMimeHandler.DownloadWithStatus] Clearing existing directory: {directory.FullName}");
            }

            var downloadProgress = new DownloadProgress(); 
            foreach (var folderItem in folderItems)
            {
                var fullPath = Path.Combine(directory.FullName, folderItem.Name);
                DirectoryUtil.CreateAssetDirectoryIfNotExists(Path.GetDirectoryName(fullPath));
                if (!GoogleDriveUtil.MimeFileHandlers.TryGetValue(folderItem.MimeType, out var fileHandler))
                {
                    Debug.LogWarning($"[FolderMimeHandler.DownloadWithStatus] No handler for mime type: {folderItem.MimeType}, file: {folderItem.Name} (ID: {folderItem.Id})");
                    continue;
                }

                var progress = fileHandler.DownloadWithStatus(filesResource, new FileHandler.DownloadRequest()
                {
                    googleFileId = folderItem.Id,
                    rootDirectoryInfo = directory,
                    localName =  folderItem.Name,
                });
                downloadProgress.AddProgress(progress);
            }

            return downloadProgress;
        }

        public async Task<IDownloadProgress> DownloadAsync(FilesResource filesResource, FileHandler.DownloadRequest downloadRequest)
        {
            var googleFileId = downloadRequest.googleFileId;
            var subItemRequest = CreateGoogleFolderItemRequest(filesResource, googleFileId);
            var result = await subItemRequest.ExecuteAsync();
            var folderItems = result.Files;
            var directory = new DirectoryInfo(Path.Combine(downloadRequest.rootDirectoryInfo.FullName, downloadRequest.localName));
            if (directory.Exists)
            {
                directory.Delete(true);
                Debug.Log($"[FolderMimeHandler.DownloadAsync] Clearing existing directory: {directory.FullName}");
            }
            var downloadProgress = new DownloadProgress();
            foreach (var folderItem in folderItems)
            {
                var fullPath = Path.Combine(directory.FullName, folderItem.Name);
                DirectoryUtil.CreateAssetDirectoryIfNotExists(Path.GetDirectoryName(fullPath));
                if (!GoogleDriveUtil.MimeFileHandlers.TryGetValue(folderItem.MimeType, out var fileHandler))
                {
                    Debug.LogWarning($"[FolderMimeHandler.DownloadAsync] No handler for mime type: {folderItem.MimeType}, file: {folderItem.Name} (ID: {folderItem.Id})");
                    continue;
                }

                var progress = await fileHandler.DownloadAsync(filesResource, new FileHandler.DownloadRequest()
                {
                    googleFileId = folderItem.Id,
                    rootDirectoryInfo = directory,
                    localName =  folderItem.Name,
                });
                downloadProgress.AddProgress(progress);
            }

            return downloadProgress;
        }

        private FilesResource.ListRequest CreateGoogleFolderItemRequest(FilesResource filesResource, string folderId)
        {
            var request = filesResource.List();
            request.Q = $"'{folderId}' in parents";
            request.Fields = "files(id, name, mimeType)";
            return request;
        }
    }
}