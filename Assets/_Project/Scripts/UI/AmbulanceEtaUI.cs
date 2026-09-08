using System.Collections;
using TMPro;
using UnityEngine;

public class AmbulanceEtaUI : MonoBehaviour
{
    [SerializeField] private PhaseController phaseController;
    [SerializeField] private TMP_Text timerEtaText;
    [SerializeField] private GameObject ambulanceEtaPanel;

    private void OnEnable()
    {
        phaseController.OnTimeEtaChanged += UpdateEtaTimer;
    }
    
    private void OnDisable()
    {
        phaseController.OnTimeEtaChanged -= UpdateEtaTimer;
    }
    
    private void UpdateEtaTimer(int secondsLeft)
    {
        if (secondsLeft <= 0)
        {
            ambulanceEtaPanel.SetActive(false);
            return;
        }
        
        ambulanceEtaPanel.SetActive(true);
        
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;

        timerEtaText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}