using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider slider;
    public Toggle autoEndTurnToggle;
    public TMP_Text valueText;

    private const string SensitivityKey = "MouseSensitivity";
    private const string AutoEndTurnKey = "AutoEndTurn";

    private void Start()
    {
        // Initialize the slider value based on PlayerPrefs or a default value
        float sensitivityValue = PlayerPrefs.GetFloat(SensitivityKey, 2.0f);
        slider.value = sensitivityValue;
        UpdateValueText(sensitivityValue);

        // Initialize the toggle state based on PlayerPrefs or a default value
        bool autoEndTurnState = PlayerPrefs.GetInt(AutoEndTurnKey, 0) == 1;
        autoEndTurnToggle.isOn = autoEndTurnState;

        // Add listeners
        slider.onValueChanged.AddListener(UpdateSensitivity);
        autoEndTurnToggle.onValueChanged.AddListener(UpdateAutoEndTurn);
    }

    private void UpdateValueText(float value)
    {
        // Update the text to display the current sensitivity value
        valueText.text = "Sensitivity: " + value.ToString("F2");
    }

    private void UpdateSensitivity(float value)
    {
        // Update the mouse sensitivity value when the slider is moved
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
        UpdateValueText(value);
    }

    private void UpdateAutoEndTurn(bool isAutoEndTurn)
    {
        // Update the AutoEndTurn PlayerPrefs based on the toggle state
        int autoEndTurnValue = isAutoEndTurn ? 1 : 0;
        PlayerPrefs.SetInt(AutoEndTurnKey, autoEndTurnValue);
        PlayerPrefs.Save();
    }
}