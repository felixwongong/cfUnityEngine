using System;
using cfEngine;
using cfEngine.Util;

namespace cfUnityEngine.GoogleDrive
{
    public enum FileType
    {
        None,
        Folder,
        Sheet
    }
    
    public struct UrlInfo
    {
        public static UrlInfo Empty => new UrlInfo
        {
            fileType = FileType.None,
            fileId = string.Empty
        };
        
        public FileType fileType;
        public string fileId;
    }
    
    public static class GoogleDriveUtil
    {
        public static string FormLink(string driveFileId)
        {
            return $"https://docs.google.com/spreadsheets/d/{driveFileId}";
        }
        
        public static Res<UrlInfo, Exception> ParseUrl(string driveLink)
        {
            if (string.IsNullOrEmpty(driveLink))
                return Res<UrlInfo, Exception>.Err(new Exception("Drive link cannot be null or empty."));

            var url = new Uri(driveLink);
            var localPath = url.LocalPath;
            var pathSegments = localPath.Split('/');
            return ParseSegments(pathSegments);
        }

        public static Res<UrlInfo, Exception> ParseSegments(ReadOnlyMemory<string> segments)
        {
            var segmentsSpan = segments.Span;
            for (var i = 0; i < segmentsSpan.Length; i++)
            {
                var segment = segmentsSpan[i];
                switch (segment)
                {
                    case "folders":
                        return Res.Ok(
                                new UrlInfo()
                                {
                                    fileType = FileType.Folder,
                                    fileId = segmentsSpan[i + 1]
                                }
                            );
                    case "spreadsheets":
                        return Res.Ok(
                                new UrlInfo()
                                {
                                    fileType = FileType.Sheet,
                                    fileId = segmentsSpan[i + 2]
                                }
                            );
                    default:
                        continue;
                }
            }
            return Res.Err<UrlInfo>(new ArgumentOutOfRangeException(nameof(segments), $"Unsupported segment: {string.Join('/', segments)}. "));
        }
    }
}