using OrganismSim.PlayerActions;
using UnityEngine;
using UnityEngine.UI;

public sealed class ActionMenuController : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private PhaseController phaseController;
    [SerializeField] private Button juiceButton;
    [SerializeField] private Button waterButton;
    [SerializeField] private Button biscuitButton;
    [SerializeField] private Button glucagonButton;
    [SerializeField] private Button insulinButton;
    [SerializeField] private Button helpStand;
    [SerializeField] private Button helpSit;
    [SerializeField] private Button callAmbulanceButton;
    [SerializeField] private Text feedbackText;

    private void Start()
    {
        juiceButton.onClick.AddListener(() => Execute(new GiveJuice()));
        waterButton.onClick.AddListener(() => Execute(new GiveWater()));
        biscuitButton.onClick.AddListener(() => Execute(new GiveBiscuit()));
        glucagonButton.onClick.AddListener(() => Execute(new GiveGlucagon()));
        insulinButton.onClick.AddListener(() => Execute(new GiveInsulin()));
        helpStand.onClick.AddListener(() => Execute(new HelpStand()));
        helpSit.onClick.AddListener(() => Execute(new HelpSit()));
        callAmbulanceButton.onClick.AddListener(() => phaseController.CallAmbulance());
    }

    private void Update()
    {
        bool interactable = phaseController.IsActivePhase;
        juiceButton.interactable = interactable;
        waterButton.interactable = interactable;
        biscuitButton.interactable = interactable;
        glucagonButton.interactable = interactable;
        insulinButton.interactable = interactable;
        helpStand.interactable = interactable;
        helpSit.interactable = interactable;
    }

    private void Execute(IPlayerAction action)
    {
        var result = patientController.TryExecuteAction(action);
        feedbackText.text = result.Message;
    }
}