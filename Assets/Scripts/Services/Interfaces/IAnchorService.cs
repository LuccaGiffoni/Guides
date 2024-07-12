using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Services.Interfaces
{
    public interface IAnchorService
    {
        Task CreateAnchorAsync(Vector3 position);
        Task DeleteAnchorAsync(Guid anchorId);
        Task<List<Guid>> LoadAnchorAsync();

        event Action<Guid> AnchorCreated;
        event Action<Guid> AnchorDeleted;
        event Action<List<Guid>> AnchorLoaded;
    }
}