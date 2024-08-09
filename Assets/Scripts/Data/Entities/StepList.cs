using System.Collections.Generic;
using Data.Settings;
using Newtonsoft.Json;

namespace Data.Entities
{
    public class StepList
    {
        public List<Step> Steps { get; set; } = new();
        
        public void Save(string persistentDataPath)
        {
            var filePath = persistentDataPath + DirectoryPaths.Steps;
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(filePath, json);
        }
        
        public static StepList Read(string persistentDataPath)
        {
            var filePath = persistentDataPath + DirectoryPaths.Steps;
            var file = System.IO.File.ReadAllText(filePath);
            
            return JsonConvert.DeserializeObject<StepList>(file);
        }
    }
}