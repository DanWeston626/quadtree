using System;
using UnityEngine;
using UnityEngine.UI;

namespace ui {
    public class DebugSlider : DebugElement {

        /// <summary>
        /// Slider variable name
        /// </summary>
        public Text ident;

        /// <summary>
        /// UnityEngine.UI slider GO reference
        /// </summary>
        public Slider slider;

        private void Awake() {
            Debug.Assert(ident != null);
            Debug.Assert(slider != null);
        }

        /// <summary>
        /// Initialise/construct slider instance
        /// </summary>        
        public void Init (string ident, int value, int min, int max, Action<int> onValueChanged) {
            this.ident.text = string.Format("{0}:\t{1}", ident, value);
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
            slider.wholeNumbers = true;
            slider.onValueChanged.AddListener((float x) => {
                onValueChanged.Invoke(Mathf.CeilToInt(x));
                this.ident.text = string.Format("{0}:\t{1}", ident, x);
            });
        }

    }
}
