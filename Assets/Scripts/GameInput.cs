using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }

    private InputActions inputActions;

    public event EventHandler OnInteractAction;
    public event EventHandler OnChangeCam; 
    public event EventHandler OnAttack;
    public event EventHandler OnAbility1Performed;
    public event EventHandler OnAbility2Performed;

    private void Awake()
    {
        if (!Instance)
            Instance = this;

        inputActions = new InputActions();

        inputActions.Player.Enable();
        inputActions.Interactions.Enable();

        inputActions.Interactions.Interact.performed += Interaction_performed;
        inputActions.Interactions.ChangeCam.performed += ChangeCam_performed;
        inputActions.Player.Fire.performed += Fire_performed;
        inputActions.Interactions.Ability1.performed += Ability1_performed;
        inputActions.Interactions.Ability2.performed += Ability2_performed;
    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        OnAbility1Performed?.Invoke(this, EventArgs.Empty);
    }
    private void Ability2_performed(InputAction.CallbackContext obj)
    {
        OnAbility2Performed?.Invoke(this, EventArgs.Empty);
    }

    private void Fire_performed(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    private void ChangeCam_performed(InputAction.CallbackContext obj)
    {
        OnChangeCam?.Invoke(this, EventArgs.Empty);
    }

    private void Interaction_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        inputActions.Interactions.Interact.performed -= Interaction_performed;
        inputActions.Interactions.ChangeCam.performed -= ChangeCam_performed;
        inputActions.Dispose();
    }
    public InputActions GetInputActions()
    {
        return inputActions;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
    public Vector2 GetMouseLook()
    {
        Vector2 lookVector = inputActions.Player.MouseLook.ReadValue<Vector2>();
        return lookVector;

    }
}
