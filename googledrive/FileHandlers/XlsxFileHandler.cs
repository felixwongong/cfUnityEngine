using System.IO;
using System.Threading.Tasks;
using Google.Apis.Download;
using Google.Apis.Drive.v3;

namespace cfUnityEngine.GoogleDrive
{
    public struct XlsxFileHandler : FileHandler
    {
        public IDownloadProgress DownloadWithStatus(FilesResource filesResource, in FileHandler.DownloadRequest downloadRequest)
        {
            var fullPath = Path.Combine(downloadRequest.rootDirectoryInfo.FullName, downloadRequest.localName);
            using var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
            var fileId = downloadRequest.googleFileId;
            var request = filesResource.Get(fileId);
            return request.DownloadWithStatus(fileStream);
        }

        public async Task<IDownloadProgress> DownloadAsync(FilesResource filesResource, FileHandler.DownloadRequest downloadRequest)
        {
            var fullPath = Path.Combine(downloadRequest.rootDirectoryInfo.FullName, downloadRequest.localName);
            await using var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
            var fileId = downloadRequest.googleFileId;
            var request = filesResource.Get(fileId);
            return await request.DownloadAsync(fileStream);
        }
    }
}