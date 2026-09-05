using System;
using UnityEngine;
using OrganismSim.Core;
using OrganismSim.Diagnoses;

public sealed class ScenarioController : MonoBehaviour
{
    [Header("Scenario")]
    [SerializeField] private DiagnosisType diagnosis;
    [SerializeField] private string patientName = "баба Стоянка";

    [Header("References")]
    [SerializeField] private PatientController patientController;
    
    public event Action OnPatientReady;
    
    private void Start()
    {
        Patient patient = new Patient(patientName);
        DiagnosisFactory.AddProfile(diagnosis, patient);
        patientController.Initialize(patient);
        OnPatientReady?.Invoke();
    }
}