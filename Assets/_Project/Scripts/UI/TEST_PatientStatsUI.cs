using System.Collections.Generic;
using System.Linq;
using OrganismSim.Core;
using UnityEngine;
using TMPro;

public sealed class TEST_PatientStatsUI : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private TMP_Text parametersText;
    [SerializeField] private TMP_Text conditionsText;

    private void OnEnable()
    {
        if (patientController == null) return;
        patientController.OnTick += UpdateParameters;
        patientController.OnTick += UpdateConditions;
    }

    private void OnDisable()
    {
        if (patientController == null) return;
        patientController.OnTick -= UpdateParameters;
        patientController.OnTick -= UpdateConditions;
    }

    private void UpdateParameters(Patient patient)
    {
        List<string> localizedParameters = new();
        int pending = patient.Physiology.Snapshot().Count;

        foreach (var parameter in patient.Physiology.Snapshot())
        {
            LocalizationService.Instance.GetParameterName(parameter.Key, (translatedParameter) =>
            {
                localizedParameters.Add($"{translatedParameter}: {parameter.Value:F1}");
                pending--;

                if (pending == 0) parametersText.text = string.Join("\n", localizedParameters);
            });
        }
    }

    private void UpdateConditions(Patient patient)
    {
        List<string> localizedConditions = new();
        int pending = patient.Pathology.ActiveConditions().Count;

        foreach (var cond in patient.Pathology.ActiveConditions())
        {
            LocalizationService.Instance.GetConditionName(cond.Key, (translatedCondition) =>
            {
                localizedConditions.Add($"{translatedCondition}: {cond.Value:F1}");
                pending--;

                if (pending == 0) conditionsText.text = string.Join("\n", localizedConditions);
            });
        }
    }
}