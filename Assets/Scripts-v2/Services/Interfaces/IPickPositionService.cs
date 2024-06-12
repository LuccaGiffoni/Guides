using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts_v2.Data.Models;
using UnityEngine;

namespace Scripts_v2.Services.Interfaces
{
    public interface IPickPositionService
    {
        Task CreatePickPositionAsync(int stepIndex, Vector3 position);
        Task<List<PickPositionData>> LoadPickPositionsAsync();
        Task SavePickPositionsAsync(List<PickPositionData> pickPositions);
        event Action<PickPositionData> PickPositionCreated;
        event Action<List<PickPositionData>> PickPositionsLoaded;
    }
}