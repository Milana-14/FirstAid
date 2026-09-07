using System.Collections.Generic;
using OrganismSim.Core;
using UnityEngine;

public sealed class ScenarioResultUI : MonoBehaviour
{
    [SerializeField] private PhaseController phaseController;
    private readonly ComplicationEngine _complicationEngine = new();

    private void Awake()
    {
        phaseController.OnScenarioEnded += HandleScenarioEnded;
    }
    
    private void OnDestroy()
    {
        phaseController.OnScenarioEnded -= HandleScenarioEnded;
    }

    private void HandleScenarioEnded(ScenarioOutcome outcome, Patient patient) // усложнения
    {
        IReadOnlyList<string> complications = _complicationEngine.Evaluate(patient);
        
        if (outcome == ScenarioOutcome.AmbulanceArrived)
        {
            // добър резултат
        }
        else if (outcome == ScenarioOutcome.Died)
        {
            // лош резултат
        }
        else if (outcome ==  ScenarioOutcome.Stabilized) // усложнения
        {
            // Прекрасен резултат
        }
    }
}