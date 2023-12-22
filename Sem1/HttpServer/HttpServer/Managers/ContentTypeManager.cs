using System;
namespace HttpServer
{
    public static class ContentTypeManager
    {
        private static Dictionary<string, string> mimeTypeMapping = new Dictionary<string, string>()
        {
            { ".html", "text/html; charset=utf-8" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".mp3", "audio/mpeg" },
            { ".wav", "audio/wav"},
            { ".mp4", "video/mp4"},
            { ".avi", "video/x-msvideo" },
            { ".xml", "application/xml"}
        };

        public static string? GetContentType(string path) =>
            mimeTypeMapping
                .TryGetValue(Path.GetExtension(path).ToLower(), out string? type)
                ? type
                : null;
    }
}