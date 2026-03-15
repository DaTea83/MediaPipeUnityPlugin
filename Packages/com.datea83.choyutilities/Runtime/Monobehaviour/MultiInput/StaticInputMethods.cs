using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace EugeneC.Utilities {
    public static partial class UtilityMethods {
        public static string InterfaceToStringName(Type type, string replaced = null, string replacedwith = "") {
            string Name = "";
            if (replaced != null)
                Name = type.Name.Replace(replaced, replacedwith);
            return Name.Substring(1);
        }

        public static void BindPlayerAction<T>(T playerAction, InputActionMap map)
            where T : IControlBinder {
            System.Type type = playerAction.InputInterface;
            foreach (var method in type.GetMethods()) {
                string actionName = method.Name.Substring(2);
                InputAction action = map.FindAction(actionName);

                if (action != null) {
                    var delegates =
                        (Action<InputAction.CallbackContext>)method.CreateDelegate(
                            typeof(Action<InputAction.CallbackContext>), playerAction);
                    action.Reset();
                    action.started += delegates;
                    action.performed += delegates;
                    action.canceled += delegates;
                    Debug.Log($"Bind {actionName} to {action}");
                }
                else
                    Debug.Log($"Unable to find {actionName} in {map}");
            }
        }

        public static EControlSchemeEnum GetDeviceType(InputDevice device) {
            EControlSchemeEnum scheme = EControlSchemeEnum.Gamepad;

            if (device is Gamepad)
                scheme = EControlSchemeEnum.Gamepad;
            else if (device is Keyboard)
                scheme = EControlSchemeEnum.Keyboard;

            return scheme;
        }

        public static string GetControlType(this EControlSchemeEnum eControl) {
            string mode = null;
            switch (eControl) {
                case EControlSchemeEnum.Keyboard:
                    mode = nameof(Keyboard);
                    break;
                case EControlSchemeEnum.Gamepad:
                    mode = nameof(Gamepad);
                    break;
            }

            return mode;
        }
    }
}

#endif