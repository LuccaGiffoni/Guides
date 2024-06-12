using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts_v2.Data.Models;
using UnityEngine;

namespace Scripts_v2.Data.Access
{
    public interface IDatabaseRepository
    {
        Task<List<Operation>> GetAllOperationsAsync();
        Task<List<Step>> GetStepsForOperationAsync(int operationId);
        Task<bool> UpdateOperationAnchorUuidAsync(int operationId, Guid anchorUuid);
        Task<bool> DeleteOperationAnchorUuidAsync(int operationId);
        Task<string> DeletePickPosition(int stepId);
        Task<string> PostPickPositionAsync(int stepId, Transform pickPosition);
    }
}