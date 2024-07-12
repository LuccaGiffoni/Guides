using Data.Entities;
using Data.Responses;

namespace Services.Interfaces
{
    public interface IUIService
    {
        void Init(Response<StepList> response);
        void Refresh(Response<int> response);
    }
}