using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Faust
{
    [RequireComponent(typeof(Slider))]
    public class FaustSlider : FaustParameter
    {
        Slider slider;

        void Start()
        {
            slider = GetComponent<Slider>();

            slider.value = this.Parameter;
            slider.minValue = this.ParameterMin;
            slider.maxValue = this.ParameterMax;

            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        void OnDestroy()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(OnSliderChanged);
            }
        }

        void OnValidate()
        {
            var label = GetComponentInChildren<Text>();
            if (!string.IsNullOrWhiteSpace(parameterName) && label != null)
            {
                label.text = parameterName;
            }
        }

        void OnSliderChanged(float value)
        {
            this.Parameter = value;
        }

    }
}