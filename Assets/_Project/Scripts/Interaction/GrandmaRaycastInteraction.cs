using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using OrganismSim.PlayerActions;

public sealed class GrandmaRaycastInteraction : MonoBehaviour
{
    [Header("Ray")]
    [SerializeField] private Transform rayHolder;
    [SerializeField] private float rayLength = 2.5f;
    [SerializeField] private LayerMask collisionLayers = Physics.DefaultRaycastLayers;

    [Header("Controllers")]
    [SerializeField] private GrandmaPostureController postureController;
    [SerializeField] private PatientController patientController;
    [SerializeField] private PhaseController phaseController;

    [Header("UX")]
    [SerializeField] private Image pointer;
    [SerializeField] private TextMeshProUGUI sign;
    [SerializeField] private Color grandmaPromptColor = Color.yellow;

    [Header("Keys")]
    [SerializeField] private Key sitKey = Key.E;
    [SerializeField] private Key lieKey = Key.F;
    [SerializeField] private Key standKey = Key.R;

    private string _currentPromptKey;

    private void Awake()
    {
        if (rayHolder == null && Camera.main != null)
            rayHolder = Camera.main.transform;

        HidePrompt();
    }

    private void Update()
    {
        if (phaseController != null && !phaseController.IsActivePhase)
        {
            HidePrompt();
            return;
        }

        if (rayHolder == null || postureController == null || Keyboard.current == null)
        {
            HidePrompt();
            return;
        }

        if (!Physics.Raycast(rayHolder.position, rayHolder.forward, out RaycastHit hit, rayLength, collisionLayers))
        {
            HidePrompt();
            return;
        }

        Transform target = hit.transform;
        bool isGrandma = target == postureController.transform || target.IsChildOf(postureController.transform);
        if (!isGrandma)
        {
            HidePrompt();
            return;
        }

        HandleGrandmaHit();
    }

    private void HandleGrandmaHit()
    {
        if (pointer != null)
            pointer.color = grandmaPromptColor;

        bool canSit = postureController.CanHelpSit(rayHolder, out _, out string sitReason);
        bool canLie = postureController.CanHelpLieDown(rayHolder, out _, out string lieReason);
        bool canStand = !postureController.IsForcedDown && postureController.CurrentPosture != CharacterPosture.Standing;

        string promptKey = null;
        if (canSit)
            promptKey = "Prompt_Grandma_HelpSit";
        else if (canLie)
            promptKey = "Prompt_Grandma_HelpLie";
        else if (canStand)
            promptKey = "Prompt_Grandma_HelpStand";
        else if (sitReason == "no_seat_nearby")
            promptKey = "Prompt_Grandma_NoSeat";
        else if (lieReason == "no_bed_nearby")
            promptKey = "Prompt_Grandma_NoBed";
        else
            promptKey = "Prompt_Grandma_Default";

        ShowPrompt(promptKey);

        if (canSit && Keyboard.current[sitKey].wasPressedThisFrame)
        {
            if (postureController.TryHelpSit(rayHolder))
                patientController?.TryExecuteAction(new HelpSit());
        }
        else if (canLie && Keyboard.current[lieKey].wasPressedThisFrame)
        {
            postureController.TryHelpLieDown(rayHolder);
        }
        else if (canStand && Keyboard.current[standKey].wasPressedThisFrame)
        {
            postureController.StandUp();
            patientController?.TryExecuteAction(new HelpStand());
        }
    }

    private void ShowPrompt(string key, params object[] args)
    {
        if (sign == null) return;
        if (key == _currentPromptKey) return;

        _currentPromptKey = key;

        if (LocalizationService.Instance != null)
        {
            LocalizationService.Instance.GetLocalizedStringWithArgs("UI_Table", key, args, localizedText =>
            {
                if (sign != null && _currentPromptKey == key)
                    sign.text = localizedText;
            });
        }
        else
        {
            sign.text = key;
        }
    }

    private void HidePrompt()
    {
        if (sign == null) return;
        if (string.IsNullOrEmpty(_currentPromptKey)) return;

        _currentPromptKey = string.Empty;
        sign.text = string.Empty;
    }

    private void OnDrawGizmosSelected()
    {
        if (rayHolder == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(rayHolder.position, rayHolder.forward * rayLength);
    }
}