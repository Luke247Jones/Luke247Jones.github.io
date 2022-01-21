using System;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Slider[] sliders;
    public Text[] sliderTexts;

    private void Start()
    {
        UpdateSliderValues();
    }

    public void SliderValueChanged(Slider slider) {
        GlobalVariables.mouseSensitivity = (float)Math.Round(slider.value, 2);
        UpdateSliderValues();
    }

    private void UpdateSliderValues() {
        foreach (Slider slider in sliders)
        {
            slider.value = GlobalVariables.mouseSensitivity;
        }

        foreach (Text text in sliderTexts)
        {
            text.text = GlobalVariables.mouseSensitivity.ToString("0.00");
        }
    }
}
