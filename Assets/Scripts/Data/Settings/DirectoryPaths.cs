using UnityEngine;

namespace Data.Settings
{
    public static class DirectoryPaths
    {
        public const string OperationToManage = "/manager/operation.json";
        public const string OperationToOperate = "/operator/operation.json";
        public const string Steps = "/steps.json";
        public const string Anchor = "/anchor.json";
        
        #region Response
        
        public static readonly string SuccessFilePath = Application.persistentDataPath + "/logs/success.txt";
        public static readonly string ErrorFilePath = Application.persistentDataPath + "/logs/error.txt";
        
        #endregion
    }
}