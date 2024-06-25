using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts_v2.Data.Models;
using Scripts_v2.Data.Responses;
using UnityEngine;

namespace Scripts_v2.Data.Access
{
    public interface IDatabaseRepository
    {
        Task<Response<List<Operation>>> GetAllOperationsAsync();
        Task<Response<List<Step>>> GetStepsForOperationAsync(int operationId);
        Task<Response<bool>> UpdateOperationAnchorUuidAsync(int operationId, Guid anchorUuid);
        Task<Response<bool>> DeleteOperationAnchorUuidAsync(int operationId);
        Task<Response<bool>> DeletePickPositionAsync(int stepId);
        Task<Response<bool>> PostPickPositionAsync(int stepId, Transform pickPosition);
    }
}