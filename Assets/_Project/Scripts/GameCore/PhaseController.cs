using System;
using System.Collections;
using OrganismSim.Core;
using UnityEngine;

public sealed class PhaseController : MonoBehaviour
{
    public static PhaseController Instance { get; private set; }

    [SerializeField] private PatientController patientController;
    [SerializeField] private ScenarioController scenarioController;
    [SerializeField] private int ambulanceEtaSeconds = 300; // balance

    public bool IsActivePhase { get; private set; }
    private bool _scenarioEnded;
    public bool AmbulanceCalled { get; private set; }
    private int _accumulatedEtaSeconds;
    private bool _ambulanceArrived;
    public event Action<int> OnTimeEtaChanged;

    public event Action<ScenarioOutcome, Patient> OnScenarioEnded;

    private void Awake()
    {
        Instance = this;
        scenarioController.OnPatientReady += HandlePatientReady;
    }

    private void HandlePatientReady()
    {
        Patient patient = patientController.Patient;

        patient.OnPatientDied += HandlePatientDied;
        patient.OnPatientStabilized += HandlePatientStabilized;
        
        StartCoroutine(ScenarioSequence());
    }
    
    private void OnDestroy()
    {
        if (scenarioController != null) scenarioController.OnPatientReady -= HandlePatientReady;

        if (patientController != null && patientController.Patient != null)
        {
            patientController.Patient.OnPatientDied -= HandlePatientDied;
            patientController.Patient.OnPatientStabilized -= HandlePatientStabilized;
        }
    }

    private IEnumerator ScenarioSequence() // i love this part (inspired by Inscryption)
    {
        yield return StartCoroutine(IntroPhase());
        yield return StartCoroutine(ActivePhase());
    }

    private IEnumerator IntroPhase()
    {
        patientController.IsPaused = true;
        yield return new WaitForSeconds(1f); // balance (за интро диалог или UI, нз)
    }

    private IEnumerator ActivePhase()
    {
        IsActivePhase = true;
        patientController.IsPaused = false;

        while (!_scenarioEnded) yield return null;

        IsActivePhase = false;
        patientController.IsPaused = true;
    }

    public void CallAmbulance()
    {
        if (AmbulanceCalled) return;
        AmbulanceCalled = true;
        StartCoroutine(AmbulanceTimer());
    }

    private IEnumerator AmbulanceTimer()
    {
        _accumulatedEtaSeconds = 0;
        
        while (_accumulatedEtaSeconds <= ambulanceEtaSeconds)
        {
            yield return new WaitForSeconds(1f);
            _accumulatedEtaSeconds += 1;

            OnTimeEtaChanged?.Invoke(ambulanceEtaSeconds - _accumulatedEtaSeconds);
        }
        
        _ambulanceArrived = true;
        EndScenario(ScenarioOutcome.AmbulanceArrived);
    }
    
    private void HandlePatientDied()
    {
        EndScenario(ScenarioOutcome.Died);
    }

    private void HandlePatientStabilized()
    {
        EndScenario(ScenarioOutcome.Stabilized);
    }
    
    private void EndScenario(ScenarioOutcome outcome)
    {
        if (_scenarioEnded) return;
        
        _scenarioEnded = true;
        patientController.IsPaused = true;
        IsActivePhase = false;

        OnScenarioEnded?.Invoke(outcome, patientController.Patient);
    }
}