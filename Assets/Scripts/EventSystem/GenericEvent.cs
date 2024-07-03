using System.Collections.Generic;
using EventSystem.Enums;

namespace EventSystem
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