using System.Collections.Generic;
using OrganismSim.Core;
using TMPro;
using UnityEngine;

public sealed class ScenarioResultUI : MonoBehaviour
{
    [SerializeField] private PhaseController phaseController;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject panelObject;
    
    private void OnEnable()
    {
        phaseController.OnScenarioEnded += HandleScenarioEnded;
    }

    private void OnDisable()
    {
        phaseController.OnScenarioEnded -= HandleScenarioEnded;
    }

    private void HandleScenarioEnded(ScenarioOutcome outcome, Patient patient, DeathCause deathCause, IReadOnlyList<string> complications)
    {
        panelObject.SetActive(true);
        
        string key = outcome switch
        {
            ScenarioOutcome.AmbulanceArrived => complications.Count == 0 ? "outcome_ambulance_clean" : "outcome_ambulance_complications",
            ScenarioOutcome.Stabilized => complications.Count == 0 ? "outcome_stabilized_clean" : "outcome_stabilized_complications",
            ScenarioOutcome.Died => "outcome_died",
            _ => string.Empty
        };
        
        var args = new object[]
        {
            new {
                patientName = patient.Name,
                deathCause = deathCause.ToString(),
                complications = string.Join(", ", complications)
            }
        };

        LocalizationService.Instance.GetLocalizedStringWithArgs("UI_Table", key, args, (translatedText) =>
        {
            resultText.text = translatedText;
        });
    }
}