using System;
using System.Collections.Generic;
using Data.Responses;
using Data.Settings;
using Newtonsoft.Json;
using Utils;

namespace Data.Entities
{
    [Serializable]
    public class Operation
    {
        public int OperationID;
        public Guid AnchorUuid;
        public string Description;
        public int Status;

        public bool GetStatus() => Status == 1;
        
        public GenericResponse<bool> Save(string persistentDataPath, OperationType type)
        {
            try
            {
                var filePath = type == OperationType.Manager
                ? persistentDataPath + DirectoryPaths.OperationToManage
                : persistentDataPath + DirectoryPaths.OperationToOperate;

                return GenericResponse<bool>.SuccessResponse(true, "Operation saved with success!");
            }
            catch (Exception e)
            {
                var exceptions = new List<string> { e.Message };
                    
                return GenericResponse<bool>.FailureResponse(exceptions, "Operation cannot be saved with success.");
            }
        }

        public static GenericResponse<Operation> Read(string persistentDataPath, OperationType type)
        {
            try
            {
                var filePath = type == OperationType.Manager
                    ? persistentDataPath + DirectoryPaths.OperationToManage
                    : persistentDataPath + DirectoryPaths.OperationToOperate;

                var file = System.IO.File.ReadAllText(filePath);
            
                return GenericResponse<Operation>.SuccessResponse(JsonConvert.DeserializeObject<Operation>(file), "Operation has been read with success!");
            }
            catch (Exception e)
            {
                var exceptions = new List<string> { e.Message };

                return GenericResponse<Operation>.FailureResponse(exceptions, "The operation cannot be read.");
            }
        }
    }

    public enum OperationType
    {
        Manager,
        Operator
    }
    
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
            => reader.TokenType == JsonToken.Null ? Guid.Empty : new Guid(reader.Value?.ToString() ?? throw new InvalidOperationException());

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString());
    }
}
