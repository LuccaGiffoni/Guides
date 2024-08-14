using System.Collections;
using Data.Enums;

namespace Services.Interfaces
{
    public interface IPopupService
    { 
        void SendMessageToUser(string message, EPopupType type);
        IEnumerator HidePopupAfterSeconds(float seconds);
    }
}