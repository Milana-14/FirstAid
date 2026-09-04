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
        var snapshot = patient.Physiology.Snapshot();
        parametersText.text = string.Join("\n", snapshot.Select(p => $"{p.Key}: {p.Value}"));
    }
    
    private void UpdateConditions(Patient patient)
    {
        var snapshot = patient.Pathology.ActiveConditions();
        conditionsText.text = string.Join("\n", snapshot.Select(p => $"{p.Key}: {p.Value}"));
    }
}