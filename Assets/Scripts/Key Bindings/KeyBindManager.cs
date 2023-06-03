using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class KeyBindManager : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public event EventHandler OnBindingRebind;

    [SerializeField] private TextMeshProUGUI sprint, jump, interact,interactAlternate, diary, changeCam, vote, ability1, ability2, status, inventory;
    [SerializeField] private Button sprintButton, jumpButton, interactButton, interactAlternateButton, diaryButton, changeCamButton, voteButton, ability1Button, ability2Button, statusButton, inventoryButton;
    [SerializeField] Transform rebindPanel;
    InputActions inputActions;
    public enum Binding
    {
        Sprint,
        Jump,
        Diary,
        Interact,
        InteractAlternate,
        ChangeCam,
        Vote,
        Ability1,
        Ability2,
        Status,
        Inventory
    }
    private void Awake()
    {
        sprintButton.onClick.AddListener(() => { RebindBinding(Binding.Sprint); });
        jumpButton.onClick.AddListener(() => { RebindBinding(Binding.Jump); });
        interactButton.onClick.AddListener(() => { RebindBinding(Binding.Interact); });
        interactAlternateButton.onClick.AddListener(() => { RebindBinding(Binding.InteractAlternate); });
        diaryButton.onClick.AddListener(() => { RebindBinding(Binding.Diary); });
        changeCamButton.onClick.AddListener(() => { RebindBinding(Binding.ChangeCam); });
        voteButton.onClick.AddListener(() => { RebindBinding(Binding.Vote); });
        ability1Button.onClick.AddListener(() => { RebindBinding(Binding.Ability1); });
        ability2Button.onClick.AddListener(() => { RebindBinding(Binding.Ability2); });
        statusButton.onClick.AddListener(() => { RebindBinding(Binding.Status); });
        inventoryButton.onClick.AddListener(() => { RebindBinding(Binding.Inventory); });
    }

    // Start is called before the first frame update
    void Start()
    {
        inputActions = GameInput.Instance.GetInputActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        UpdateVisual();

    }
    private void UpdateVisual()
    {
        sprint.text = inputActions.Player.Sprint.bindings[0].ToDisplayString();
        jump.text = inputActions.Player.Jump.bindings[0].ToDisplayString();
        interact.text = inputActions.Interactions.Interact.bindings[0].ToDisplayString();
        interactAlternate.text = inputActions.Interactions.InteractAlternate.bindings[0].ToDisplayString();
        diary.text = inputActions.Interactions.Diary.bindings[0].ToDisplayString();
        changeCam.text = inputActions.Interactions.ChangeCam.bindings[0].ToDisplayString();
        vote.text = inputActions.Interactions.Vote.bindings[0].ToDisplayString();
        ability1.text = inputActions.Interactions.Ability1.bindings[0].ToDisplayString();
        ability2.text = inputActions.Interactions.Ability2.bindings[0].ToDisplayString();
        status.text = inputActions.Interactions.Status.bindings[0].ToDisplayString();
        inventory.text = inputActions.Interactions.Inventory.bindings[0].ToDisplayString();
    }
    internal void RebindBinding(Binding binding)
    {
        inputActions.Player.Disable();
        inputActions.Interactions.Disable();
        rebindPanel.gameObject.SetActive(true);
        InputAction inputAction = null;

        switch (binding)
        {
            default:
            case Binding.Sprint:
                inputAction = inputActions.Player.Sprint;
                break;
            case Binding.Jump:
                inputAction = inputActions.Player.Jump;
                break;
            case Binding.Diary:
                inputAction = inputActions.Interactions.Diary;
                break;
            case Binding.Interact:
                inputAction = inputActions.Interactions.Interact;
                break;
            case Binding.InteractAlternate:
                inputAction = inputActions.Interactions.InteractAlternate;
                break;
            case Binding.ChangeCam:
                inputAction = inputActions.Interactions.ChangeCam;
                break;
            case Binding.Vote:
                inputAction = inputActions.Interactions.Vote;
                break;
            case Binding.Ability1:
                inputAction = inputActions.Interactions.Ability1;
                break;
            case Binding.Ability2:
                inputAction = inputActions.Interactions.Ability2;
                break;
            case Binding.Status:
                inputAction = inputActions.Interactions.Status;
                break;
            case Binding.Inventory:
                inputAction = inputActions.Interactions.Inventory;
                break;

        }
        inputAction.PerformInteractiveRebinding(0)
            .OnComplete(callback =>
            {
                callback.Dispose();
                inputActions.Player.Enable();
                inputActions.Interactions.Enable();
                UpdateVisual();
                rebindPanel.gameObject.SetActive(false);
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, inputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
    private void Save()
    {
        Debug.Log("saved");
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
        PlayerPrefs.Save();
    }
}
