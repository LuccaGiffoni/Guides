using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts_v2.Utilities
{
    public enum PopupType
    {
        Error,
        Warning,
        Info
    }
    
    public class PopupManager : MonoBehaviour
    {
        [Header("Popup")]
        [SerializeField, Tooltip("This will be the user's log")] public GameObject popupCanvas;
        [SerializeField, Tooltip("This will be the image of the user's log")] public Image image;
        [SerializeField, Tooltip("This will be the text of the user's log")] public TextMeshProUGUI text;
        
        [Header("Popup Settings")]
        [SerializeField, Tooltip("This will be the duration of the user's log")] private float popupDuration = 3f;

        public void SendMessageToUser(string message, PopupType type)
        {
            image.color = type switch
            {
                PopupType.Error => Color.red,
                PopupType.Warning => Color.yellow,
                PopupType.Info => Color.green,
                _ => image.color
            };

            text.text = message;
            popupCanvas.SetActive(true);

            StartCoroutine(HidePopupAfterSeconds(popupDuration));
        }

        private IEnumerator HidePopupAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            popupCanvas.SetActive(false);
        }
    }
}