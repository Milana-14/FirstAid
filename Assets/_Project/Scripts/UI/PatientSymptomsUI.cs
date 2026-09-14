using System.Collections.Generic;
using OrganismSim.Core;
using TMPro;
using UnityEngine;

public sealed class PatientSymptomsUI : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private TMP_Text symptomsText;

    private void OnEnable()
    {
        patientController.OnTick += UpdateSymptoms;
    }

    private void OnDisable()
    {
        patientController.OnTick -= UpdateSymptoms;
    }

    private void UpdateSymptoms(Patient patient)
    {
        if (patient.ActiveSymptoms == null || patient.ActiveSymptoms.Count == 0)
        {
            LocalizationService.Instance.GetLocalizedString("UI_Table", "no_symptoms", (text) => 
            {
                symptomsText.text = text;
            });
            return;
        }

        List<string> localizedSymptoms = new();
        int pending = patient.ActiveSymptoms.Count;

        foreach (var symptom in patient.ActiveSymptoms)
        {
            LocalizationService.Instance.GetSymptomName(symptom, (translatedSymptom) =>
            {
                localizedSymptoms.Add(translatedSymptom);
                pending--;

                if (pending == 0) symptomsText.text = string.Join("\n", localizedSymptoms);
            });
        }
    }
}