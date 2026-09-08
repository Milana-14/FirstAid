using System.Collections.Generic;
using OrganismSim.Core;
using TMPro;
using UnityEngine;

public sealed class ScenarioResultUI : MonoBehaviour
{
    [SerializeField] private PhaseController phaseController;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject textObject;
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
        
        textObject.SetActive(true);
        
        if (outcome == ScenarioOutcome.AmbulanceArrived)
        {
            // добър резултат
            resultText.text = "Линейката е пристигнала на време благодарение на твоята помощ";

            if (complications.Count <= 0)
            {
                resultText.text += ". Бабата даже не е развила никакви усложнения, ти си истинка сигма. ";
            }
            else if (complications.Count > 0)
            {
                resultText.text += $". Обаче {patient.Name} е {string.Join(", ", complications)}.";
            }
        }
        else if (outcome == ScenarioOutcome.Died)
        {
            // лош резултат
            resultText.text = "Ти си идиот. ";
        }
        else if (outcome ==  ScenarioOutcome.Stabilized) // усложнения
        {
            // Прекрасен резултат
            resultText.text = "Браво1 Ти си сигма";
            
            if (complications.Count <= 0)
            {
                resultText.text += ". Бабата даже не е развила никакви усложнения, ти си истинка сигма.";
            }
            else if (complications.Count > 0)
            {
                resultText.text += $". Обаче {patient.Name} е {string.Join(", ", complications)}.";
            }
        }
    }
}