using UnityEngine;
using TMPro;

public sealed class SimulationTimerUI : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private TMP_Text timerText;

    private void OnEnable()
    {
        patientController.OnTimeChanged += UpdateTimer;
    }
    
    private void OnDisable()
    {
        patientController.OnTimeChanged -= UpdateTimer;
    }
    
    private void UpdateTimer(int elapsedSeconds)
    {
        int minutes = elapsedSeconds / 60;
        int seconds = elapsedSeconds % 60;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}