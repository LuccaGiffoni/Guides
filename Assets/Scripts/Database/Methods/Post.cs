using System;
using System.Threading.Tasks;
using Database.Settings;
using Language;
using UnityEngine;
using UnityEngine.Networking;

namespace Database.Methods
{
    public static class Post
    {
        [Tooltip("Update the Operation's Spatial Anchor UUID in the database.")]
        public static async Task<bool> UpdateOperationAnchorUuidAsync(int operationId, Guid anchorUuid)
        {
            WWWForm form = new();
            form.AddField("operationId", operationId.ToString());
            form.AddField("anchorUuid", anchorUuid.ToString());
            
            var updateAnchorUrl = ConnectionSettings.apiUrl + "?action=update_spatialAnchor";
            using var uwr = UnityWebRequest.Post(updateAnchorUrl, form);
            uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(DatabaseLogMessages.serverConnectionFailed + uwr.result);
                return false;
            }

            Debug.Log(DatabaseLogMessages.serverConnectionSucceeded);
            return true;
        }
        
        [Tooltip("Clear the Spatial Anchor's UUID in the database.")]
        public static async Task<bool> ClearOperationAnchorUuidAsync(int operationId)
        {
            WWWForm form = new();
            form.AddField("operationId", operationId.ToString());
            
            var clearAnchorUrl = ConnectionSettings.apiUrl + "?action=clear_spatialAnchor";
            using var uwr = UnityWebRequest.Post(clearAnchorUrl, form);
            uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(DatabaseLogMessages.serverConnectionFailed + uwr.result);
                return false;
            }

            return true;
        }

        [Tooltip("Save PickPosition by step on database.")]
        public static async Task<bool> SavePickPositionToDatabase(int stepId, Transform pickPosition)
        {
            WWWForm form = new();
            form.AddField("stepId", stepId.ToString());
            form.AddField("PX", pickPosition.transform.position.x.ToString("n4").Replace(",", "."));
            form.AddField("PY", pickPosition.transform.position.y.ToString("n4").Replace(",", "."));
            form.AddField("PZ", pickPosition.transform.position.z.ToString("n4").Replace(",", "."));
            form.AddField("RX", pickPosition.rotation.x.ToString("n4").Replace(",", "."));
            form.AddField("RY", pickPosition.rotation.y.ToString("n4").Replace(",", "."));
            form.AddField("RZ", pickPosition.rotation.z.ToString("n4").Replace(",", "."));
            form.AddField("RW", pickPosition.rotation.w.ToString("n4").Replace(",", "."));
            form.AddField("SX", pickPosition.localScale.x.ToString("n4").Replace(",", "."));
            form.AddField("SY", pickPosition.localScale.y.ToString("n4").Replace(",", "."));
            form.AddField("SZ", pickPosition.localScale.z.ToString("n4").Replace(",", "."));
            
            var updatePickPositionString = ConnectionSettings.apiUrl + "?action=update_step";
            using var uwr = UnityWebRequest.Post(updatePickPositionString, form);
            uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(DatabaseLogMessages.serverConnectionFailed + uwr.result);
                return false;
            }

            Debug.Log(DatabaseLogMessages.serverConnectionSucceeded);
            Debug.Log(uwr.downloadHandler.text);
            return true;
        }
        
        private static Task SendWebRequestAsync(UnityWebRequest uwr)
        {
            var tcs = new TaskCompletionSource<bool>();
            uwr.SendWebRequest().completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}