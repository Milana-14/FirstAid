using System;
using UnityEngine;
using OrganismSim.Core;
using OrganismSim.Diagnoses;
using OrganismSim.PlayerActions;

public sealed class PatientController : MonoBehaviour
{
    private Patient _patient;
    private float _accumulatedSeconds;

    public event Action<Patient> OnTick;
    public event Action<ActionResult> OnActionResult;

    public void Initialize(DiagnosisType diagnosis)
    {
        _patient = new Patient("Стоянка");
        DiagnosisFactory.AddProfile(diagnosis, _patient);
    }

    private void Update()
    {
        _accumulatedSeconds += Time.deltaTime;
        while (_accumulatedSeconds >= 1f)
        {
            _patient.Tick(1);
            _accumulatedSeconds -= 1f;
            OnTick?.Invoke(_patient);
        }
    }

    public ActionResult TryExecuteAction(IPlayerAction action)
    {
        var result = action.Execute(_patient);
        OnActionResult?.Invoke(result);
        return result;
    }
}