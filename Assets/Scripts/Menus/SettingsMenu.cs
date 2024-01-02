using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider slider;
    public TMP_Text valueText;

    private void Start()
    {
        // Initialize the slider value based on PlayerPrefs or a default value
        float sensitivityValue = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);
        slider.value = sensitivityValue;
        UpdateValueText(sensitivityValue);

        // Add a listener to the slider's OnValueChanged event
        slider.onValueChanged.AddListener(UpdateSensitivity);
    }

    private void UpdateValueText(float value)
    {
        // Update the text to display the current sensitivity value
        valueText.text = "Sensitivity: " + value.ToString("F2");
    }

    private void UpdateSensitivity(float value)
    {
        // Update the mouse sensitivity value when the slider is moved
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save(); // Save the value to PlayerPrefs
        UpdateValueText(value);
    }
}
