using Google.Apis.Drive.v3.Data;

namespace cfUnityEngine.GoogleDrive
{
    public static class GoogleDriveFileExtension
    {
        public static bool isGoogleMimeType(this File file)
        {
            return file.MimeType != null && file.MimeType.StartsWith("application/vnd.google-apps");
        }

        public static string getExportMimeType(this File file)
        {
            if (!isGoogleMimeType(file))
            {
                return file.MimeType;
            }

            switch (file.MimeType)
            {
                case "application/vnd.google-apps.document":
                    // Google Docs → DOCX
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                case "application/vnd.google-apps.spreadsheet":
                    // Google Sheets → XLSX
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                case "application/vnd.google-apps.presentation":
                    // Google Slides → PPTX
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";

                case "application/vnd.google-apps.drawing":
                    // Google Drawings → PNG
                    return "image/png";

                // These types can't be exported
                case "application/vnd.google-apps.folder":
                case "application/vnd.google-apps.form":
                case "application/vnd.google-apps.site":
                case "application/vnd.google-apps.shortcut":
                case "application/vnd.google-apps.jam":
                case "application/vnd.google-apps.map":
                    return null;

                default:
                    // Not a Google Workspace file, or unknown type
                    return null;
            }
        }
    }
}