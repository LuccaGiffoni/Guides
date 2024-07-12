using System.Collections;
using Data.Enums;
using Helper;
using Services.Implementations;

namespace Services.Interfaces
{
    public interface IPopupService
    { 
        void SendMessageToUser(string message, EPopupType type);
        IEnumerator HidePopupAfterSeconds(float seconds);
    }
}