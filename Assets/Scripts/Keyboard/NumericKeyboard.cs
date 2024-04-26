using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Keyboard
{
    public class NumericKeyboard : ValidatedMonoBehaviour
    {
        [SerializeField] public InputField inputField;

        public void OnButtonClick(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    AppendToInput(buttonID.ToString());
                    break;
                case 1:
                    AppendToInput(buttonID.ToString());
                    break;
                case 2:
                    AppendToInput(buttonID.ToString());
                    break;
                case 3:
                    AppendToInput(buttonID.ToString());
                    break;
                case 4:
                    AppendToInput(buttonID.ToString());
                    break;
                case 5:
                    AppendToInput(buttonID.ToString());
                    break;
                case 6:
                    AppendToInput(buttonID.ToString());
                    break;
                case 7:
                    AppendToInput(buttonID.ToString());
                    break;
                case 8:
                    AppendToInput(buttonID.ToString());
                    break;
                case 9:
                    AppendToInput(buttonID.ToString());
                    break;
                case 10:
                    AppendToInput(".");
                    break;
                case 11:
                    RemoveLastCharacter();
                    break;
                default:
                    break;
            }
        }

        private void AppendToInput(string value)
        {
            inputField.text += value;
        }

        private void RemoveLastCharacter()
        {
            if (inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }
        }

        public string GetServerAddress()
        {
            return inputField.text;
        }
    }
}
