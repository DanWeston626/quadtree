using UnityEngine;
using System.Collections.Generic;

namespace ui {
    public class DebugController : MonoBehaviour {

        /// <summary>
        /// Singleton 
        /// </summary>
        public static DebugController instance;

        /// <summary>
        /// Root UI object to init objects
        /// </summary>
        public GameObject root;

        [Header ("Templates")]
        /// <summary>
        /// DebugSlider template
        /// </summary>
        public DebugSlider slider;

        /// <summary>
        /// DebugButton template
        /// </summary>
        public DebugButton button;

        public void Awake() {
            instance = this; // super lazy...

            Debug.Assert(slider != null);
            Debug.Assert(button != null);
        }

        public T Create<T> (T template) where T : DebugElement {
            T s = Instantiate<T>(template, root.transform);
            s.gameObject.SetActive(true);
            return s;
        }
    }
}
