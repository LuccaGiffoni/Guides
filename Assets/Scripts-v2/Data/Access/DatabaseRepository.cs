using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Settings;
using Newtonsoft.Json;
using Scripts_v2.Data.Models;
using Scripts_v2.Data.Responses;
using Scripts_v2.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Scripts_v2.Data.Access
{
    public class DatabaseRepository : IDatabaseRepository
    {
        // Get all operations from the database
        public async Task<Response<List<Operation>>> GetAllOperationsAsync()
        {
            using var uwr = UnityWebRequest.Get(ConnectionSettings.apiUrl + "?action=get_operations");
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                return Response<List<Operation>>.Failure(uwr.error, "Error while fetching operations.");
            }

            try
            {
                var jsonResponse = uwr.downloadHandler.text;
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new GuidConverter());
                var operations = JsonConvert.DeserializeObject<OperationList>("{\"operations\":" + jsonResponse + "}", settings)?.Operations;

                if (operations == null || operations.Count == 0)
                {
                    return Response<List<Operation>>.Failure("No operations returned from the server.", "Warning: No data.");
                }

                return Response<List<Operation>>.Success(operations, "Operations fetched successfully.");
            }
            catch (Exception e)
            {
                return Response<List<Operation>>.Failure(e.Message, "Failed to deserialize operations.");
            }
        }
        
        public async Task<Response<List<Step>>> GetStepsForOperationAsync(int operationId)
        {
            var stepsUrl = ConnectionSettings.apiUrl + "?action=get_steps&operationId=" + operationId;
            using var uwr = UnityWebRequest.Get(stepsUrl);
            await SendWebRequestAsync(uwr);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                return Response<List<Step>>.Failure(uwr.error, "Error while fetching steps.");
            }

            try
            {
                var jsonResponse = uwr.downloadHandler.text;
                var steps = JsonConvert.DeserializeObject<StepList>("{\"steps\":" + jsonResponse + "}")?.Steps;

                if (steps == null || steps.Count == 0)
                {
                    return Response<List<Step>>.Failure("No steps returned from the server.", "Warning: No data.");
                }

                return Response<List<Step>>.Success(steps, "Steps fetched successfully.");
            }
            catch (Exception e)
            {
                return Response<List<Step>>.Failure(e.Message, "Failed to deserialize steps.");
            }
        }
        
        public async Task<Response<bool>> UpdateOperationAnchorUuidAsync(int operationId, Guid anchorUuid)
        {
            WWWForm form = new();
            form.AddField("operationId", operationId.ToString());
            form.AddField("anchorUuid", anchorUuid.ToString());
            
            var updateAnchorUrl = ConnectionSettings.apiUrl + "?action=update_spatialAnchor";
            using var uwr = UnityWebRequest.Post(updateAnchorUrl, form);
            uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            await SendWebRequestAsync(uwr);

            return uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                ? Response<bool>.Failure(uwr.error, "Error while updating anchor UUID.")
                : Response<bool>.Success(true, "Anchor UUID updated successfully.");
        }
        
        public async Task<Response<bool>> DeleteOperationAnchorUuidAsync(int operationId)
        {
            WWWForm form = new();
            form.AddField("operationId", operationId.ToString());

            var clearAnchorUrl = ConnectionSettings.apiUrl + "?action=clear_spatialAnchor";
            using var uwr = UnityWebRequest.Post(clearAnchorUrl, form);
            uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            await SendWebRequestAsync(uwr);

            return uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                ? Response<bool>.Failure(uwr.error, "Error while clearing anchor UUID.")
                : Response<bool>.Success(true, "Anchor UUID cleared successfully.");
        }

        public async Task<Response<bool>> PostPickPositionAsync(int stepId, Transform pickPosition)
        {
            try
            {
                WWWForm form = new();
                form.AddField("stepId", stepId.ToString());
                form.AddField("PX", pickPosition.transform.localPosition.x.ToString("n4").Replace(",", "."));
                form.AddField("PY", pickPosition.transform.localPosition.y.ToString("n4").Replace(",", "."));
                form.AddField("PZ", pickPosition.transform.localPosition.z.ToString("n4").Replace(",", "."));
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

                return uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                    ? Response<bool>.Failure(uwr.error, "Error while posting pick position.")
                    : Response<bool>.Success(true, "Pick position posted successfully.");
            }
            catch (Exception e)
            {
                return Response<bool>.Failure(e.Message, "Exception occurred while posting pick position.");
            }
        }
        
        public async Task<Response<bool>> DeletePickPositionAsync(int stepId)
        {
            try
            {
                WWWForm form = new();
                form.AddField("stepId", stepId.ToString());
                form.AddField("PX", 0);
                form.AddField("PY", 0);
                form.AddField("PZ", 0);
                form.AddField("RX", 0);
                form.AddField("RY", 0);
                form.AddField("RZ", 0);
                form.AddField("RW", 0);
                form.AddField("SX", 0);
                form.AddField("SY", 0);
                form.AddField("SZ", 0);

                var updatePickPositionString = ConnectionSettings.apiUrl + "?action=update_step";
                using var uwr = UnityWebRequest.Post(updatePickPositionString, form);
                uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                await SendWebRequestAsync(uwr);

                if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    return Response<bool>.Failure(uwr.error, "Error while deleting pick position.");
                }

                return Response<bool>.Success(true, "Pick position deleted successfully.");
            }
            catch (Exception e)
            {
                return Response<bool>.Failure(e.Message, "Exception occurred while deleting pick position.");
            }
        }

        private static Task SendWebRequestAsync(UnityWebRequest uwr)
        {
            var tcs = new TaskCompletionSource<bool>();
            uwr.SendWebRequest().completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
