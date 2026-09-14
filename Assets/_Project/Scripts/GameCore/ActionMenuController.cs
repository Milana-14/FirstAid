using OrganismSim.PlayerActions;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ActionMenuController : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private PhaseController phaseController;

    private void Update()
    {
        if (!phaseController.IsActivePhase) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) Execute(new GiveJuice());
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) Execute(new GiveWater());
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) Execute(new GiveBiscuit());
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) Execute(new GiveGlucagon());
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) Execute(new GiveInsulin());
        else if (Keyboard.current.digit6Key.wasPressedThisFrame) Execute(new HelpStand());
        else if (Keyboard.current.digit7Key.wasPressedThisFrame) Execute(new HelpSit());
        else if (Keyboard.current.digit8Key.wasPressedThisFrame) phaseController.CallAmbulance();
    }

    private void Execute(IPlayerAction action)
    {
        patientController.TryExecuteAction(action);
    }
}