using System;

namespace cfUnityEngine.GoogleDrive
{
    public static class GoogleDriveUtil
    {
        public static string FormLink(string driveFileId)
        {
            return $"https://docs.google.com/spreadsheets/d/{driveFileId}";
        }
        
        public static string ExtractFileId(string driveLink)
        {
            if (string.IsNullOrEmpty(driveLink))
            {
                return string.Empty;
            }
            
            var parts = driveLink.Split("/spreadsheets/d/");
            if (parts.Length < 2)
            {
                throw new ArgumentException($"Invalid Google Drive link: {driveLink}");
            }
            
            var fileIdPart = parts[1].Split("/")[0];
            if (string.IsNullOrEmpty(fileIdPart))
            {
                throw new ArgumentException($"Invalid Google Drive link: {driveLink}");
            }
            return fileIdPart;
        }
    }
}