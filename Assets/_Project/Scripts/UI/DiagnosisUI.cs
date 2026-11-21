using TMPro;
using UnityEngine;

public sealed class DiagnosisUI : MonoBehaviour
{
    [SerializeField] private ScenarioController scenarioController;
    [SerializeField] private GameObject diagnosisPanel;
    [SerializeField] private TMP_Text diagnosisText;

    private void OnEnable()
    {
        scenarioController.OnPatientReady += HandlePatientReady;
    }

    private void OnDisable()
    {
        scenarioController.OnPatientReady -= HandlePatientReady;
    }

    private void HandlePatientReady()
    {
        bool showDiagnosis = GameSettings.Difficulty == GameDifficulty.Easy;

        diagnosisPanel.SetActive(showDiagnosis);

        if (!showDiagnosis) return;

        diagnosisText.text = scenarioController.Diagnosis.ToString();
    }
}