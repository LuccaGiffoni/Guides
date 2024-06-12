using Scripts_v2.Data.Models;
using Scripts_v2.Utilities;

namespace Scripts_v2.Services.Interfaces
{
    public interface IUIService
    {
        void UpdateUIForOperation(Operation operation);
        void UpdateUIForStep(Step step);
        void DisplayMessage(string message, PopupType type);
    }
}