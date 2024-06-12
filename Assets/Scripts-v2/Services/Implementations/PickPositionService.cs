using Scripts_v2.Data.Models;
using Scripts_v2.Services.Interfaces;

namespace Scripts_v2.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class PickPositionService : IPickPositionService
    {
        private readonly List<PickPositionData> pickPositions = new();

        public event Action<PickPositionData> PickPositionCreated;
        public event Action<List<PickPositionData>> PickPositionsLoaded;

        public async Task CreatePickPositionAsync(int stepIndex, Vector3 position)
        {
            var pickPosition = new PickPositionData
            {
                Id = Guid.NewGuid(),
                StepIndex = stepIndex,
                Position = position
            };
            pickPositions.Add(pickPosition);
            
            // Simulate asynchronous operation
            await Task.Delay(500);
            PickPositionCreated?.Invoke(pickPosition);
        }

        public async Task<List<PickPositionData>> LoadPickPositionsAsync()
        {
            // Simulate loading pick positions
            await Task.Delay(500);
            PickPositionsLoaded?.Invoke(pickPositions);
            return pickPositions;
        }

        public async Task SavePickPositionsAsync(List<PickPositionData> pickPositions)
        {
            // Simulate saving pick positions
            await Task.Delay(500);
        }
    }

}