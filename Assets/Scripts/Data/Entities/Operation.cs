using System;
using Data.Settings;
using Newtonsoft.Json;

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
        
        public void Save(string persistentDataPath)
        {
            var filePath = persistentDataPath + DirectoryPaths.Operation;
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(filePath, json);
        }

        public static Operation Read(string persistentDataPath)
        {
            var filePath = persistentDataPath + DirectoryPaths.Operation;
            var file = System.IO.File.ReadAllText(filePath);
            
            return JsonConvert.DeserializeObject<Operation>(file);
        }
    }
    
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.Null ? Guid.Empty : new Guid(reader.Value?.ToString() ?? throw new InvalidOperationException());
        }

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
