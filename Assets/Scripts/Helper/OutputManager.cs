using System;
using System.IO;
using UnityEngine;

namespace Helper
{
    public static class OutputManager
    {
        private static readonly string DocumentsPath = Path.Combine(Application.persistentDataPath, "Logs");
        private static Guid SessionId = Guid.Empty;
        
        public static readonly bool IsActive = false;
        
        public static string CreateOutputFile()
        {
            if (SessionId == Guid.Empty && IsActive) SessionId = Guid.NewGuid();
            if (!IsActive) return string.Empty;
            
            if (!Directory.Exists(DocumentsPath))
                Directory.CreateDirectory(DocumentsPath);

            var fileName = $"{SessionId}.txt";

            var filePath = Path.Combine(DocumentsPath, fileName);

            File.WriteAllText(filePath, string.Empty);

            return filePath;
        }

        public static void AppendLineToFile(string line)
        {
            if (SessionId == Guid.Empty && IsActive)
                CreateOutputFile();
            
            if(IsActive)
                File.AppendAllText(DocumentsPath, line + Environment.NewLine);
        }
    }
}