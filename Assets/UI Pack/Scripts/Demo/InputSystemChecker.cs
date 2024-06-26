﻿using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace HungwNguyen.MUIP
{
    public class InputSystemChecker : MonoBehaviour
    {
        private static InputSystemChecker instance;
        private void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Awake()
        {
            
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            InputSystemUIInputModule tempModule = gameObject.GetComponent<InputSystemUIInputModule>();

            if (tempModule == null)
            {
                Debug.LogError("<b>[Modern UI Pack]</b> Input System is enabled, but <b>'Input System UI Input Module'</b> is missing. " +
                    "Select the event system object, and click the <b>'Replace'</b> button.");
            }
#endif
        }
    }
}