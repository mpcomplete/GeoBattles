//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.1
//     from Assets/Input/Inputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Inputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Inputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Inputs"",
    ""maps"": [
        {
            ""name"": ""GamePlay"",
            ""id"": ""a714ef05-b690-4bba-bf82-7ddb545766b9"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""98a8dd2e-71c3-4fcd-ba52-954ddf98dcb5"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""257fe56e-a2d4-4d25-b0d9-810dcda246e3"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Bomb"",
                    ""type"": ""Button"",
                    ""id"": ""dcca2586-e3f3-4c26-bf59-e777fb9b9057"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NextShot"",
                    ""type"": ""Button"",
                    ""id"": ""f26a8e2f-f27a-4d5a-92c6-8845f060fd4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""a7144e97-1c8a-4952-800c-5eb0f7d3b4d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseAim"",
                    ""type"": ""Value"",
                    ""id"": ""6a3bfca2-381c-439a-b289-f2bb138cc5ed"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9a2719d2-a992-4687-b6f4-3bdb526dbaa4"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""eda2d664-04d6-4b8b-8859-fb01dd429325"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""90e71c85-9c2b-4712-87d4-e53cb866ebab"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c5bd1ac3-c787-4b0d-8899-92ecc8d69d33"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1d82608d-6e87-4f79-9e8f-0eebcf57a490"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3f7e72cc-1699-4057-a4e0-bbaeaa7a65fe"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9635a383-d371-4a46-b823-5e88cd567106"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2eb1a473-fe20-4ce2-83b5-1f2b671dfe22"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Bomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca555353-1253-4bd1-86e8-850f195ed0ab"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Bomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6f633ad-c322-48cf-b0ff-9c04235127ef"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Bomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""906ab2c2-a17c-45da-8a5f-aad1d9d76001"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Bomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ba57ad4-afd8-4dc5-97a5-aadc451000bf"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextShot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47ea0b1a-dfd8-4b48-8b5e-dc5bbb5894b5"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb664b78-9425-46fb-8a08-1a6dedb8f942"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""MouseAim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""537d7927-f0b8-421b-b236-950cff99ca28"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8c86eb39-b272-4b12-a483-285fc3c6d5ef"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""ea42475b-c5fd-4c15-b42c-c2ed44ad0d3c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5285a6c6-6321-402d-8501-e72c3a948651"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8af72605-ea31-4a3f-b9d1-e7e4523f4ae6"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09b83039-e088-4aad-b801-ea5f0004dcf1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard;Controller"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""MouseKeyboard"",
            ""bindingGroup"": ""MouseKeyboard"",
            ""devices"": []
        }
    ]
}");
        // GamePlay
        m_GamePlay = asset.FindActionMap("GamePlay", throwIfNotFound: true);
        m_GamePlay_Move = m_GamePlay.FindAction("Move", throwIfNotFound: true);
        m_GamePlay_Aim = m_GamePlay.FindAction("Aim", throwIfNotFound: true);
        m_GamePlay_Bomb = m_GamePlay.FindAction("Bomb", throwIfNotFound: true);
        m_GamePlay_NextShot = m_GamePlay.FindAction("NextShot", throwIfNotFound: true);
        m_GamePlay_Pause = m_GamePlay.FindAction("Pause", throwIfNotFound: true);
        m_GamePlay_MouseAim = m_GamePlay.FindAction("MouseAim", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Move = m_UI.FindAction("Move", throwIfNotFound: true);
        m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // GamePlay
    private readonly InputActionMap m_GamePlay;
    private List<IGamePlayActions> m_GamePlayActionsCallbackInterfaces = new List<IGamePlayActions>();
    private readonly InputAction m_GamePlay_Move;
    private readonly InputAction m_GamePlay_Aim;
    private readonly InputAction m_GamePlay_Bomb;
    private readonly InputAction m_GamePlay_NextShot;
    private readonly InputAction m_GamePlay_Pause;
    private readonly InputAction m_GamePlay_MouseAim;
    public struct GamePlayActions
    {
        private @Inputs m_Wrapper;
        public GamePlayActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_GamePlay_Move;
        public InputAction @Aim => m_Wrapper.m_GamePlay_Aim;
        public InputAction @Bomb => m_Wrapper.m_GamePlay_Bomb;
        public InputAction @NextShot => m_Wrapper.m_GamePlay_NextShot;
        public InputAction @Pause => m_Wrapper.m_GamePlay_Pause;
        public InputAction @MouseAim => m_Wrapper.m_GamePlay_MouseAim;
        public InputActionMap Get() { return m_Wrapper.m_GamePlay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GamePlayActions set) { return set.Get(); }
        public void AddCallbacks(IGamePlayActions instance)
        {
            if (instance == null || m_Wrapper.m_GamePlayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GamePlayActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Aim.started += instance.OnAim;
            @Aim.performed += instance.OnAim;
            @Aim.canceled += instance.OnAim;
            @Bomb.started += instance.OnBomb;
            @Bomb.performed += instance.OnBomb;
            @Bomb.canceled += instance.OnBomb;
            @NextShot.started += instance.OnNextShot;
            @NextShot.performed += instance.OnNextShot;
            @NextShot.canceled += instance.OnNextShot;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @MouseAim.started += instance.OnMouseAim;
            @MouseAim.performed += instance.OnMouseAim;
            @MouseAim.canceled += instance.OnMouseAim;
        }

        private void UnregisterCallbacks(IGamePlayActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Aim.started -= instance.OnAim;
            @Aim.performed -= instance.OnAim;
            @Aim.canceled -= instance.OnAim;
            @Bomb.started -= instance.OnBomb;
            @Bomb.performed -= instance.OnBomb;
            @Bomb.canceled -= instance.OnBomb;
            @NextShot.started -= instance.OnNextShot;
            @NextShot.performed -= instance.OnNextShot;
            @NextShot.canceled -= instance.OnNextShot;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @MouseAim.started -= instance.OnMouseAim;
            @MouseAim.performed -= instance.OnMouseAim;
            @MouseAim.canceled -= instance.OnMouseAim;
        }

        public void RemoveCallbacks(IGamePlayActions instance)
        {
            if (m_Wrapper.m_GamePlayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGamePlayActions instance)
        {
            foreach (var item in m_Wrapper.m_GamePlayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GamePlayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GamePlayActions @GamePlay => new GamePlayActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_Move;
    private readonly InputAction m_UI_Submit;
    public struct UIActions
    {
        private @Inputs m_Wrapper;
        public UIActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_UI_Move;
        public InputAction @Submit => m_Wrapper.m_UI_Submit;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Submit.started += instance.OnSubmit;
            @Submit.performed += instance.OnSubmit;
            @Submit.canceled += instance.OnSubmit;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Submit.started -= instance.OnSubmit;
            @Submit.performed -= instance.OnSubmit;
            @Submit.canceled -= instance.OnSubmit;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("MouseKeyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IGamePlayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnBomb(InputAction.CallbackContext context);
        void OnNextShot(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnMouseAim(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
    }
}
