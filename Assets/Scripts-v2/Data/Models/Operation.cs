using System;
using Newtonsoft.Json;

namespace Scripts_v2.Data.Models
{
    [Serializable]
    public class Operation
    {
        public int OperationID;
        public Guid AnchorUuid = Guid.Empty;
        public string Description;
        public int Status;

        public bool GetStatus() => Status == 1;
    }
    
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return Guid.Empty;
            }

            return new Guid(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
