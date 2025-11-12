/// 
/// © vipercode corp
/// 2024
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace viperOSK
{
    public class OSK_LongPressPacket
    {
        public string character;
        public OSK_KeyCode keyCode;
        public GameObject keyObj;

        public string keyPressType;

        public OSK_LongPressPacket(string c, OSK_KeyCode code, GameObject obj, string pressType)
        {
            character = c;
            keyCode = code;
            keyObj = obj;
            keyPressType = pressType;
        }
    }

    public class OSK_Settings : MonoBehaviour
    {

        public static OSK_Settings instance { get; private set; }

        [System.NonSerialized]
        public bool hasAccentedConsole;

        /// <summary>
        /// how long in secs before longpress action is invoked
        /// </summary>
        public float longPressDelay = 1f;

#if UNITY_2019
        public UE2019_LongPress longPressAction;
#else

        public UnityEvent<OSK_LongPressPacket> longPressAction;
#endif

        /// <summary>
        /// type in the physical keyboard layout of your keyboard starting top-left and moving right then down
        /// </summary>
        [TextArea(1, 8)]
        public string physicalKeyboardLayout = "QWERTYUIOP[]\nASDFGHJKL;'\nZXCVBNM,./";

        public Dictionary<KeyCode, OSK_KeyCode> physicalKeyboardMap = new Dictionary<KeyCode, OSK_KeyCode>();

        /// <summary>
        /// remaps physical keyboard to match on-screen only applies for letters and numbers only. 
        /// if top layout of the OSK row has letters than it starts with the top left key on physical keyboard (Q in a QWERTY keyboard)
        /// </summary>
        public bool remapPhysicalKeyboard;

        public void SetLongPressAction(UnityAction<OSK_LongPressPacket> action)
        {
            longPressAction.AddListener(action);
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;

            }
            
        }



    }
}
