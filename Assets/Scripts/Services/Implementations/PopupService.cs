using System.Collections;
using Data.Enums;
using Services.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Implementations
{
    public class PopupService : MonoBehaviour, IPopupService
    {
        [Header("Popup")]
        [SerializeField, Tooltip("This will be the user's log")] public GameObject popupCanvas;
        [SerializeField, Tooltip("This will be the image of the user's log")] public Image image;
        [SerializeField, Tooltip("This will be the text of the user's log")] public TextMeshProUGUI text;
        
        [Header("Popup Settings")]
        [SerializeField, Tooltip("This will be the duration of the user's log")] private float popupDuration = 3f;

        public void SendMessageToUser(string message, EPopupType type)
        {
            image.color = type switch
            {
                EPopupType.Error => Color.red,
                EPopupType.Warning => Color.yellow,
                EPopupType.Info => Color.green,
                _ => image.color
            };

            text.text = message;
            popupCanvas.SetActive(true);

            StartCoroutine(HidePopupAfterSeconds(popupDuration));
        }

        public IEnumerator HidePopupAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            popupCanvas.SetActive(false);
        }
    }
}