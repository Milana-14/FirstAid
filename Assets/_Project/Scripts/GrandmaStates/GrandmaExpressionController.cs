using System.Collections.Generic;
using UnityEngine;
using OrganismSim.Core;

public sealed class GrandmaExpressionController : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;

    private int _blendShapeBlink = -1;
    private int _blendShapeSad = -1;
    private int _blendShapeAngry = -1;
    private int _blendShapeOpenMouth = -1;

    [SerializeField] private float lerpSpeed = 5f;

    private float _targetBlink;
    private float _targetSad;
    private float _targetAngry;
    private float _targetOpenMouth;

    private float _blinkTimer;
    private float _nextBlinkTime = 3f;
    private bool _isAutoBlinking;

    private void Start()
    {
        if (faceMeshRenderer == null)
            faceMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        if (faceMeshRenderer != null && faceMeshRenderer.sharedMesh != null)
        {
            _blendShapeBlink = faceMeshRenderer.sharedMesh.GetBlendShapeIndex("Blink");
            _blendShapeSad = faceMeshRenderer.sharedMesh.GetBlendShapeIndex("Sad");
            _blendShapeAngry = faceMeshRenderer.sharedMesh.GetBlendShapeIndex("Angry");
            _blendShapeOpenMouth = faceMeshRenderer.sharedMesh.GetBlendShapeIndex("Open_Mouth");
        }

        if (patientController != null)
        {
            patientController.OnTick += HandlePatientTick;
        }
    }

    private void OnDestroy()
    {
        if (patientController != null)
        {
            patientController.OnTick -= HandlePatientTick;
        }
    }

    private void Update()
    {
        if (faceMeshRenderer == null || faceMeshRenderer.sharedMesh == null) return;

        HandleAutoBlink();
        ApplyLerpedWeights();
    }

    private void HandlePatientTick(Patient patient)
    {
        if (patient == null || patient.ActiveSymptoms == null) return;

        _targetSad = 0f;
        _targetAngry = 0f;
        _targetOpenMouth = 0f;
        _targetBlink = 0f;

        foreach (var symptom in patient.ActiveSymptoms)
        {
            switch (symptom)
            {
                case SymptomType.Pain:
                case SymptomType.Headache:
                case SymptomType.Dizziness:
                case SymptomType.Weakness:
                case SymptomType.Apathy:
                    _targetSad = Mathf.Max(_targetSad, 80f);
                    break;

                case SymptomType.ShortnessOfBreath:
                    _targetSad = Mathf.Max(_targetSad, 50f);
                    _targetOpenMouth = Mathf.Max(_targetOpenMouth, 60f);
                    break;
                
                case SymptomType.Restlessness:
                case SymptomType.Agitation:
                case SymptomType.Palpitations:
                    _targetAngry = Mathf.Max(_targetAngry, 50f);
                    break;

                case SymptomType.Panic:
                case SymptomType.Anxiety:
                    _targetAngry = Mathf.Max(_targetAngry, 70f);
                    break;
                    
                case SymptomType.Aggression:
                    _targetAngry = Mathf.Max(_targetAngry, 90f);
                    break;

                case SymptomType.Nausea:
                    _targetSad = Mathf.Max(_targetSad, 60f);
                    _targetOpenMouth = Mathf.Max(_targetOpenMouth, 40f);
                    break;
                case SymptomType.Vomiting:
                    _targetSad = Mathf.Max(_targetSad, 100f);
                    _targetOpenMouth = Mathf.Max(_targetOpenMouth, 100f);
                    break;

                case SymptomType.Drowsiness:
                case SymptomType.Stupor:
                    _targetBlink = Mathf.Max(_targetBlink, 70f);
                    break;

                case SymptomType.Unresponsive:
                    _targetBlink = Mathf.Max(_targetBlink, 100f);
                    _targetOpenMouth = Mathf.Max(_targetOpenMouth, 20f);
                    break;
            }
        }
    }

    private void HandleAutoBlink()
    {
        _blinkTimer += Time.deltaTime;
        if (_blinkTimer >= _nextBlinkTime)
        {
            StartCoroutine(PerformBlink());
            _blinkTimer = 0f;
            _nextBlinkTime = Random.Range(2.55f, 5.5f);
        }
    }
    
    private IEnumerator<float> PerformBlink()
    {
        float elapsed = 0f;
        float blinkDuration = 0.15f;
        
        float startWeight = (_blendShapeBlink != -1) ? faceMeshRenderer.GetBlendShapeWeight(_blendShapeBlink) : 0f;

        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / blinkDuration;
            
            float blinkValue = Mathf.Lerp(startWeight, 100f, Mathf.Sin(progress * Mathf.PI));
            
            if (_blendShapeBlink != -1)
            {
                faceMeshRenderer.SetBlendShapeWeight(_blendShapeBlink, blinkValue);
            }
            
            yield return 0;
        }
    }

    private void ApplyLerpedWeights()
    {
        float step = lerpSpeed * Time.deltaTime;

        if (_blendShapeBlink != -1)
        {
            float current = faceMeshRenderer.GetBlendShapeWeight(_blendShapeBlink);
            faceMeshRenderer.SetBlendShapeWeight(_blendShapeBlink, Mathf.Lerp(current, _targetBlink, step));
        }
        if (_blendShapeSad != -1)
        {
            float current = faceMeshRenderer.GetBlendShapeWeight(_blendShapeSad);
            faceMeshRenderer.SetBlendShapeWeight(_blendShapeSad, Mathf.Lerp(current, _targetSad, step));
        }
        if (_blendShapeAngry != -1)
        {
            float current = faceMeshRenderer.GetBlendShapeWeight(_blendShapeAngry);
            faceMeshRenderer.SetBlendShapeWeight(_blendShapeAngry, Mathf.Lerp(current, _targetAngry, step));
        }
        if (_blendShapeOpenMouth != -1)
        {
            float current = faceMeshRenderer.GetBlendShapeWeight(_blendShapeOpenMouth);
            faceMeshRenderer.SetBlendShapeWeight(_blendShapeOpenMouth, Mathf.Lerp(current, _targetOpenMouth, step));
        }
    }
}