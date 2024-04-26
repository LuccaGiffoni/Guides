using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Keyboard
{
    public class NumericKeyboardButton : ValidatedMonoBehaviour
    {
        [SerializeField, Scene] private NumericKeyboard numericKeyboard;
        [SerializeField, Self] private Button button;
    
        [SerializeField] public int buttonID;

        private void Start()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            numericKeyboard.OnButtonClick(buttonID);
        }
    }
}
