using System.Collections.Generic;
using Scripts_v2.Data.Models.Enums;

namespace Scripts_v2.EventSystem
{
    public class GenericEvent<T> where T: class, new()
    {
        private readonly Dictionary<EChannels, T> map = new();

        public T Get(EChannels channel = EChannels.General)
        {
            map.TryAdd(channel, new T());

            return map[channel];
        }
    }
}