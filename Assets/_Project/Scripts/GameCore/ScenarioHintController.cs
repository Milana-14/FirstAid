using OrganismSim.Core;
using OrganismSim.PlayerActions;
using UnityEngine;

public sealed class ScenarioHintController : MonoBehaviour
{
    [SerializeField] private HintUI hintUI;
    [SerializeField] private PatientController patientController;
    [SerializeField] private float idleHintDelay = 15f;

    private string[] _idleHints;
    private int _idleHintIndex;
    private float _idleTimer;
    private bool _isActive;

    private void OnEnable()
    {
        patientController.OnActionResult += HandleActionResult;
    }

    private void OnDisable()
    {
        patientController.OnActionResult -= HandleActionResult;
    }

    private void Update()
    {
        if (!_isActive || _idleHints == null) return;
        if (_idleHintIndex >= _idleHints.Length) return;

        _idleTimer += Time.deltaTime;

        if (_idleTimer < idleHintDelay) return;

        hintUI.ShowThought(_idleHints[_idleHintIndex]);
        _idleHintIndex++;
        _idleTimer = 0f;
    }

    private void HandleActionResult(ActionResult result)
    {
        NotifyProgress();
    }

    public void StartHints()
    {
        _isActive = true;
    }

    public void StopHints()
    {
        _isActive = false;
    }

    public void ShowThought(string text)
    {
        _idleTimer = 0f;
        hintUI.ShowThought(text);
    }

    public void SetIdleHints(string[] hints)
    {
        _idleHints = hints;
        _idleHintIndex = 0;
        _idleTimer = 0f;
    }

    public void NotifyProgress()
    {
        _idleTimer = 0f;
    }
}