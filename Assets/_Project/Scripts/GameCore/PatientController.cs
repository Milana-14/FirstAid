using System;
using UnityEngine;
using OrganismSim.Core;
using OrganismSim.PlayerActions;

public sealed class PatientController : MonoBehaviour
{
    public Patient Patient { get; private set; }
    private float _accumulatedSeconds;
    public int ElapsedSeconds { get; private set; }
    public bool IsPaused { get; set; }

    public bool IsInitialized { get { return Patient != null; } }

    public event Action<Patient> OnTick;
    public event Action<int> OnTimeChanged;
    public event Action<ActionResult> OnActionResult;

    public void Initialize(Patient patient)
    {
        if (patient == null) throw new ArgumentNullException("patient");
        if (IsInitialized) throw new InvalidOperationException("PatientController is already initialized.");

        Patient = patient;
        _accumulatedSeconds = 0f;
        ElapsedSeconds = 0;
    }


    private void Update()
    {
        if (IsPaused || Patient == null) return;

        _accumulatedSeconds += Time.deltaTime;

        while (_accumulatedSeconds >= 1f)
        {
            ProcessOneSecond();
            _accumulatedSeconds -= 1f;
        }
    }

    private void ProcessOneSecond()
    {
        Patient.Tick(1);
        ElapsedSeconds++;

        OnTick?.Invoke(Patient);
        OnTimeChanged?.Invoke(ElapsedSeconds);
    }

    public ActionResult TryExecuteAction(IPlayerAction action)
    {
        if (Patient == null) throw new InvalidOperationException("PatientController is not initialized.");
        if (action == null) throw new ArgumentNullException("action");

        ActionResult result = action.Execute(Patient);

        OnActionResult?.Invoke(result);

        return result;
    }
}