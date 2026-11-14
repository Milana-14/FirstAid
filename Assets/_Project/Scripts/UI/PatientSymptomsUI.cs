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

        string[] localizedSymptoms = new string[patient.ActiveSymptoms.Count];
        int pending = patient.ActiveSymptoms.Count;

        for (int i = 0; i < patient.ActiveSymptoms.Count; i++)
        {
            int index = i;

            LocalizationService.Instance.GetSymptomName(patient.ActiveSymptoms[index], (translatedSymptom) =>
            {
                localizedSymptoms[index] = translatedSymptom;
                pending--;

                if (pending == 0) symptomsText.text = string.Join("\n", localizedSymptoms);
            });
        }
    }
}