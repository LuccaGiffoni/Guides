using System;
using System.Collections.Generic;
using System.IO;
using Data.Settings;
using JetBrains.Annotations;
using UnityEngine;

namespace Utils
{
    public class GenericResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public T Data { get; set; }
        
        public static GenericResponse<T> SuccessResponse(T data, [CanBeNull] string message = "", bool logToUser = true, bool logToFile = false)
        {
            if(logToUser) Debug.Log(message);
            
            if (logToFile)
            {
                try
                {
                    var logMessage = $"{DateTime.Now}: {message}\n";
                    File.AppendAllText(DirectoryPaths.SuccessFilePath, logMessage);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"{DateTime.Now}: {message}\nErrors: {ex.Message}\n";
                    Debug.LogError($"Failed to log errors to file: {ex.Message}");
                    File.AppendAllText(DirectoryPaths.ErrorFilePath, errorMessage);
                }
            }
            
            return new GenericResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static GenericResponse<T> FailureResponse(List<string> errors, [CanBeNull] string message = "", bool logToUser = true, bool logToFile = true)
        {
            var compoundErrors = string.Join(", ", errors);

            if (logToUser) Debug.Log($"{message}\nErrors: {compoundErrors}");

            if (logToFile)
            {
                try
                {
                    var logMessage = $"{DateTime.Now}: {message}\nErrors: {compoundErrors}\n";
                    File.AppendAllText(DirectoryPaths.ErrorFilePath, logMessage);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"{DateTime.Now}: {message}\nErrors: {ex.Message}\n";
                    Debug.LogError($"Failed to log errors to file: {ex.Message}");
                    File.AppendAllText(DirectoryPaths.ErrorFilePath, errorMessage);
                }
            }
            
            return new GenericResponse<T>
            {
                Success = false,
                Errors = errors,
                Message = message,
                Data = default
            };
        }
    }
}