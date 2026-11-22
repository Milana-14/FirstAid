using System.Collections.Generic;
using UnityEngine;
using OrganismSim.Core;

public enum CharacterPosture
{
    Standing,
    Walking,
    Sitting,
    Lying
}

public sealed class GrandmaPostureController : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private PatientController patientController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform characterRoot;

    [Header("Движение и мебель")]
    [SerializeField] private float walkSpeed = 1.6f;
    [SerializeField] private List<FurnitureSeatPoint> furniturePoints = new();

    [Header("Анимация")]
    [SerializeField] private float crossFadeDuration = 0.15f;
    [SerializeField] private bool useSecondLayingVariant;

    // Имена блоков должны совпадать с названиями состояний в окне Animator
    private static readonly int StandingHash = Animator.StringToHash("Standing Idle");
    private static readonly int WalkingHash = Animator.StringToHash("Walking");
    private static readonly int SittingHash = Animator.StringToHash("Sitting Idle");
    private static readonly int Laying1Hash = Animator.StringToHash("Laying Idle 1");
    private static readonly int Laying2Hash = Animator.StringToHash("Laying Idle 2");

    public CharacterPosture CurrentPosture { get; private set; } = CharacterPosture.Standing;
    public bool IsForcedDown { get; private set; }

    private Transform _moveTarget;
    private CharacterPosture _postureAfterArrival;
    private int _lastPlayedStateHash;

    private void Awake()
    {
        if (characterRoot == null) characterRoot = transform;
        if (animator == null) animator = characterRoot.GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (patientController != null)
            patientController.OnTick += HandlePatientTick;
    }

    private void OnDisable()
    {
        if (patientController != null)
            patientController.OnTick -= HandlePatientTick;
    }

    private void Update()
    {
        if (patientController == null || !patientController.IsInitialized) return;
        if (patientController.IsPaused || !patientController.Patient.IsAlive) return;

        if (_moveTarget != null)
            MoveTowardsTarget();

        PlayPostureAnimation();
    }

    private void HandlePatientTick(Patient patient)
    {
        if (patient == null || !patient.IsAlive) return;

        bool shouldCollapse = ShouldCollapseAutomatically(patient);

        if (shouldCollapse && CurrentPosture != CharacterPosture.Lying)
        {
            _moveTarget = null;
            IsForcedDown = true;
            SetPosture(CharacterPosture.Lying, patient);
        }
        else if (!shouldCollapse && IsForcedDown)
        {
            IsForcedDown = false;
        }
    }

    private bool ShouldCollapseAutomatically(Patient patient)
    {
        if (patient.GetConsciousnessLevel() == ConsciousnessLevel.Unresponsive) return true;
        if (patient.Physiology.Get(ParameterType.Consciousness) < 15) return true;
        if (patient.Physiology.Get(ParameterType.BloodPressure) < 65) return true;
        if (patient.Physiology.Get(ParameterType.BloodOxygenLevel) < 75) return true;

        var symptoms = patient.ActiveSymptoms;
        if (symptoms != null)
        {
            if (symptoms.Contains(SymptomType.Unresponsive) ||
                symptoms.Contains(SymptomType.MuscleToneLoss) ||
                symptoms.Contains(SymptomType.LossOfSwallowingReflex))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanHelpSit(Transform referencePoint, out FurnitureSeatPoint seat, out string reasonKey)
    {
        seat = FindNearestFurniture(referencePoint, allowLieOnly: false);
        if (CurrentPosture == CharacterPosture.Sitting)
        {
            reasonKey = "already_sitting";
            return false;
        }
        if (seat == null)
        {
            reasonKey = "no_seat_nearby";
            return false;
        }
        reasonKey = null;
        return true;
    }

    public bool CanHelpLieDown(Transform referencePoint, out FurnitureSeatPoint bedOrSofa, out string reasonKey)
    {
        bedOrSofa = FindNearestFurniture(referencePoint, allowLieOnly: true);
        if (CurrentPosture == CharacterPosture.Lying && !IsForcedDown)
        {
            reasonKey = "already_lying";
            return false;
        }
        if (bedOrSofa == null)
        {
            reasonKey = "no_bed_nearby";
            return false;
        }
        reasonKey = null;
        return true;
    }

    public bool TryHelpSit(Transform referencePoint)
    {
        if (!CanHelpSit(referencePoint, out var seat, out _)) return false;

        _moveTarget = seat.Anchor;
        _postureAfterArrival = CharacterPosture.Sitting;
        SetPosture(CharacterPosture.Walking, patientController.Patient);
        return true;
    }

    public bool TryHelpLieDown(Transform referencePoint)
    {
        if (!CanHelpLieDown(referencePoint, out var bedOrSofa, out _)) return false;

        IsForcedDown = false;
        _moveTarget = bedOrSofa.Anchor;
        _postureAfterArrival = CharacterPosture.Lying;
        SetPosture(CharacterPosture.Walking, patientController.Patient);
        return true;
    }

    public void StandUp()
    {
        if (IsForcedDown) return;
        _moveTarget = null;
        SetPosture(CharacterPosture.Standing, patientController.Patient);
    }

    private void MoveTowardsTarget()
    {
        Vector3 targetPos = _moveTarget.position;
        Vector3 currentPos = characterRoot.position;
        Vector3 flatTarget = new Vector3(targetPos.x, currentPos.y, targetPos.z);
        Vector3 delta = flatTarget - currentPos;

        if (delta.sqrMagnitude <= 0.04f)
        {
            characterRoot.position = targetPos;
            characterRoot.rotation = _moveTarget.rotation;
            _moveTarget = null;
            SetPosture(_postureAfterArrival, patientController.Patient);
            return;
        }

        Vector3 dir = delta.normalized;
        characterRoot.position += dir * (walkSpeed * Time.deltaTime);

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            characterRoot.rotation = Quaternion.Slerp(characterRoot.rotation, lookRot, 10f * Time.deltaTime);
        }
    }

    private void SetPosture(CharacterPosture newPosture, Patient patient)
    {
        CurrentPosture = newPosture;
        if (patient != null)
        {
            patient.Posture = PatientPosture.Supine;
        }
    }

    private void PlayPostureAnimation()
    {
        if (animator == null) return;

        int targetHash = CurrentPosture switch
        {
            CharacterPosture.Walking => WalkingHash,
            CharacterPosture.Sitting => SittingHash,
            CharacterPosture.Lying => useSecondLayingVariant ? Laying2Hash : Laying1Hash,
            _ => StandingHash
        };

        if (targetHash != _lastPlayedStateHash)
        {
            _lastPlayedStateHash = targetHash;
            animator.CrossFade(targetHash, crossFadeDuration, 0);
        }
    }

    private FurnitureSeatPoint FindNearestFurniture(Transform referencePoint, bool allowLieOnly)
    {
        if (referencePoint == null) return null;

        FurnitureSeatPoint best = null;
        float bestDistSqr = float.MaxValue;

        foreach (var point in furniturePoints)
        {
            if (point == null) continue;
            if (allowLieOnly && !point.CanLieDown) continue;

            float maxR = point.InteractRadius;
            float distToGrandma = (point.Anchor.position - characterRoot.position).sqrMagnitude;
            float distToPlayer = (point.Anchor.position - referencePoint.position).sqrMagnitude;

            if (distToPlayer <= maxR * maxR && distToGrandma <= (maxR * 2.5f) * (maxR * 2.5f))
            {
                if (distToPlayer < bestDistSqr)
                {
                    bestDistSqr = distToPlayer;
                    best = point;
                }
            }
        }

        return best;
    }
}