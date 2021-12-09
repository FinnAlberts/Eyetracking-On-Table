// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/input/InputControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""Eyetracking"",
            ""id"": ""198e39b8-87f1-49be-b690-a5b68965f2a8"",
            ""actions"": [
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""50c91680-521d-4b6d-a69c-8fd50665588c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""50ea8b34-b83e-4691-88d6-d8a07254d1a4"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Eyetracking
        m_Eyetracking = asset.FindActionMap("Eyetracking", throwIfNotFound: true);
        m_Eyetracking_MousePosition = m_Eyetracking.FindAction("MousePosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Eyetracking
    private readonly InputActionMap m_Eyetracking;
    private IEyetrackingActions m_EyetrackingActionsCallbackInterface;
    private readonly InputAction m_Eyetracking_MousePosition;
    public struct EyetrackingActions
    {
        private @InputControls m_Wrapper;
        public EyetrackingActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MousePosition => m_Wrapper.m_Eyetracking_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Eyetracking; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(EyetrackingActions set) { return set.Get(); }
        public void SetCallbacks(IEyetrackingActions instance)
        {
            if (m_Wrapper.m_EyetrackingActionsCallbackInterface != null)
            {
                @MousePosition.started -= m_Wrapper.m_EyetrackingActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_EyetrackingActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_EyetrackingActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_EyetrackingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public EyetrackingActions @Eyetracking => new EyetrackingActions(this);
    public interface IEyetrackingActions
    {
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
