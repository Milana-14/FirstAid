using System;
using System.Collections;
using OrganismSim.Core;
using UnityEngine;

public sealed class PhaseController : MonoBehaviour
{
    public static PhaseController Instance { get; private set; }

    [SerializeField] private PatientController patientController;
    [SerializeField] private ScenarioController scenarioController;
    [SerializeField] private float ambulanceEtaSeconds = 300f;

    public bool IsActivePhase { get; private set; }
    public bool AmbulanceCalled { get; private set; }
    private bool _ambulanceArrived;

    public event Action<ScenarioOutcome> OnScenarioEnded;

    private void Awake()
    {
        Instance = this;
        scenarioController.OnPatientReady += HandlePatientReady;
    }

    private void HandlePatientReady()
    {
        StartCoroutine(ScenarioSequence());
    }

    private IEnumerator ScenarioSequence() // i fkn love this part (inspired by Inscryption)
    {
        yield return StartCoroutine(IntroPhase());
        yield return StartCoroutine(ActivePhase());
        yield return StartCoroutine(EndPhase());
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

        while (patientController.Patient.IsAlive && !_ambulanceArrived)
            yield return null;

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
        yield return new WaitForSeconds(ambulanceEtaSeconds);
        _ambulanceArrived = true;
    }

    private IEnumerator EndPhase()
    {
        var outcome = patientController.Patient.IsAlive ? ScenarioOutcome.Survived : ScenarioOutcome.Died;

        if (outcome == ScenarioOutcome.Survived)
        {
            var complications = new ComplicationEngine().Evaluate(patientController.Patient);

            // тук ще се вика UI слой за добър епилог
            yield return null; // само за сега е null
        }
        else if (outcome == ScenarioOutcome.Died)
        {
            // ще се вика лош епилог
            yield return null; // само за сега е null
        }

        OnScenarioEnded?.Invoke(outcome); // трябва ми последовател за края на сценария
    }
}