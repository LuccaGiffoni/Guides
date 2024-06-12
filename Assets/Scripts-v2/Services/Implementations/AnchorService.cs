using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts_v2.Services.Interfaces;
using UnityEngine;

namespace Scripts_v2.Services.Implementations
{
    public class AnchorService : IAnchorService
    {
        private readonly List<Guid> anchors = new();
        
        public event Action<Guid> AnchorCreated;
        public event Action<Guid> AnchorDeleted;
        public event Action<List<Guid>> AnchorLoaded;
        
        public async Task CreateAnchorAsync(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAnchorAsync(Guid anchorId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Guid>> LoadAnchorAsync()
        {
            throw new NotImplementedException();
        }
    }
}