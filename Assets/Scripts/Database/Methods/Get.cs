using System.Threading.Tasks;
using Database.Entities;
using Database.Settings;
using Helper;
using Language;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Database.Methods
{
    public static class Get
    { 
        [Tooltip("Get all operations from the database.")]
        public static async Task<OperationList> GetAllOperationsAsync(PopupManager popupManager)
        {
            using var uwr = UnityWebRequest.Get(ConnectionSettings.apiUrl + "?action=get_operations");
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                popupManager.SendMessageToUser(DatabaseLogMessages.LogErrorWhileReadingData(uwr.error), PopupType.Error);
                return null;
            }

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new GuidConverter());
            
            var jsonResponse = uwr.downloadHandler.text;
            var localReceivedOperations = JsonConvert.DeserializeObject<OperationList>("{\"operations\":" + jsonResponse + "}", settings);

            if (localReceivedOperations.Operations.Count == 0)
            {
                popupManager.SendMessageToUser(DatabaseLogMessages.noServerDataReturned, PopupType.Warning);
                return null;
            }

            popupManager.SendMessageToUser(DatabaseLogMessages.ReturnedOperations(localReceivedOperations.Operations.Count), PopupType.Info);

            return localReceivedOperations;
        }
        
        [Tooltip("Get all steps for a specific operation.")]
        public static async Task<StepList> GetStepsForOperationAsync(int operationId, PopupManager popupManager)
        {
            var stepsUrl = ConnectionSettings.apiUrl + "?action=get_steps&operationId=" + operationId;

            using var uwr = UnityWebRequest.Get(stepsUrl);
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                popupManager.SendMessageToUser(DatabaseLogMessages.LogErrorWhileReadingData(uwr.error), PopupType.Error);
                return null;
            }
            
            var jsonResponse = uwr.downloadHandler.text;
            var localReceivedSteps = JsonConvert.DeserializeObject<StepList>("{\"steps\":" + jsonResponse + "}");

            return localReceivedSteps.Steps.Count > 0 ? localReceivedSteps : null;
        }
        
        private static Task SendWebRequestAsync(UnityWebRequest uwr)
        {
            var tcs = new TaskCompletionSource<bool>();
            uwr.SendWebRequest().completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}