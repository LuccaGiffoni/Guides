using System.Threading.Tasks;
using Data.Entities;
using Data.Enums;
using Data.Settings;
using Helper;
using Messages;
using Newtonsoft.Json;
using Services.Implementations;
using UnityEngine;
using UnityEngine.Networking;

namespace Data.Methods
{
    public static class Get
    { 
        [Tooltip("Get all operations from the database.")]
        public static async Task<OperationList> GetAllOperationsAsync(PopupService popupService)
        {
            using var uwr = UnityWebRequest.Get(ConnectionSettings.apiUrl + "?action=get_operations");
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                popupService.SendMessageToUser(DatabaseLogMessages.LogErrorWhileReadingData(uwr.error), EPopupType.Error);
                return null;
            }

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new GuidConverter());
            
            var jsonResponse = uwr.downloadHandler.text;
            var localReceivedOperations = JsonConvert.DeserializeObject<OperationList>("{\"operations\":" + jsonResponse + "}", settings);

            if (localReceivedOperations.Operations.Count == 0)
            {
                popupService.SendMessageToUser(DatabaseLogMessages.noServerDataReturned, EPopupType.Warning);
                return null;
            }

            popupService.SendMessageToUser(DatabaseLogMessages.ReturnedOperations(localReceivedOperations.Operations.Count), EPopupType.Info);

            return localReceivedOperations;
        }
        
        [Tooltip("Get all steps for a specific operation.")]
        public static async Task<StepList> GetStepsForOperationAsync(int operationId, PopupService popupService)
        {
            var stepsUrl = ConnectionSettings.apiUrl + "?action=get_steps&operationId=" + operationId;

            using var uwr = UnityWebRequest.Get(stepsUrl);
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                popupService.SendMessageToUser(DatabaseLogMessages.LogErrorWhileReadingData(uwr.error), EPopupType.Error);
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