using System;
using UnityEngine;
using UnityEngine.UI;

namespace ui {
    public class DebugButton : DebugElement {

        /// <summary>
        /// Slider variable name
        /// </summary>
        public Text ident;

        /// <summary>
        /// UnityEngine.button Button GO reference
        /// </summary>
        public Button button;

        private void Awake() {
            Debug.Assert(ident != null);
            Debug.Assert(button != null);
        }

        /// <summary>
        /// Initialise/construct slider instance
        /// </summary>        
        public void Init(string ident, Action<DebugButton> onClick) {
            this.ident.text = ident;
            button.onClick.AddListener(() => {
                onClick.Invoke(this);
            });
        }

    }
}
