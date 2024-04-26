using System;
using System.Collections;
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Helper
{
    public class ClockHelper : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private TextMeshProUGUI textMesh;
        [SerializeField] private bool useSeconds;

        private void Start() => StartCoroutine(UpdateClockText());

        private IEnumerator UpdateClockText()
        {
            while (true)
            {
                var currentTime = DateTime.UtcNow.ToLocalTime();
                var timeString = useSeconds
                    ? currentTime.ToString("hh:mm:ss tt")
                    : currentTime.ToString("hh:mm tt");

                textMesh.text = timeString;

                yield return new WaitForSeconds(useSeconds ? 1 : 60);
            }
        }
    }
}