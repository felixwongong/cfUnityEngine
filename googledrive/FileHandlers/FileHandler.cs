using System.IO;
using System.Threading.Tasks;
using Google.Apis.Download;
using Google.Apis.Drive.v3;

namespace cfUnityEngine.GoogleDrive
{
    public interface FileHandler
    {
        public struct DownloadRequest
        {
            public string fileId;
        }
        
        public IDownloadProgress DownloadWithStatus(FilesResource filesResource, Stream stream, in DownloadRequest downloadRequest);
        public Task<IDownloadProgress> DownloadAsync(FilesResource filesResource, Stream stream, in DownloadRequest downloadRequest);
    }

    public struct XlsxFileHandler : FileHandler
    {
        public IDownloadProgress DownloadWithStatus(FilesResource filesResource, Stream stream, in FileHandler.DownloadRequest downloadRequest)
        {
            var fileId = downloadRequest.fileId;
            var request = filesResource.Get(fileId);
            return request.DownloadWithStatus(stream);
        }

        public Task<IDownloadProgress> DownloadAsync(FilesResource filesResource, Stream stream, in FileHandler.DownloadRequest downloadRequest)
        {
            var fileId = downloadRequest.fileId;
            var request = filesResource.Get(fileId);
            return request.DownloadAsync(stream);
        }
    }
}