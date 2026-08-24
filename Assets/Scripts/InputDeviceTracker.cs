using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Serialization;

//Returns Current Input Device
public class InputDeviceTracker : MonoBehaviour
{


    public enum CurrentInputDevice
    {
        Keyboard,
        XboxPad,
        PsPad,
    }

    [SerializeField]private CurrentInputDevice currentInputDevice;

    private void OnEnable()
    {
        InputSystem.onActionChange += SetCurrentInputDevice;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= SetCurrentInputDevice;
    }

    private void SetCurrentInputDevice(object obj, InputActionChange change)
    {
        //Only check input device if input was detected
        if (change == InputActionChange.ActionPerformed)
        {
            var action =  (InputAction)obj;
            var control = action.activeControl;
            
            if (control == null) return;

            if (control.device is Keyboard or Mouse)
            {
                currentInputDevice = CurrentInputDevice.Keyboard;
            }

            if (control.device is Gamepad)
            {
                if (Gamepad.current is XInputController 
                    || Gamepad.current.name.Contains("Xbox"))
                {
                    currentInputDevice = CurrentInputDevice.XboxPad;
                }
                
                if ( Gamepad.current is DualSenseGamepadHID 
                    || Gamepad.current is DualShock4GamepadHID
                    || Gamepad.current.name.Contains("DualSense") 
                    || Gamepad.current.name.Contains("DualShock"))
                {
                    currentInputDevice = CurrentInputDevice.PsPad; 
                } 
            }
        }
    }
    
    public CurrentInputDevice ReturnCurrentInputDevice()
    {
        print(currentInputDevice);
        return currentInputDevice;
    }
}
