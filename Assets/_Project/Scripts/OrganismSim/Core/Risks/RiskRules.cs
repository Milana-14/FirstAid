using System;
using System.Collections.Generic;
using OrganismSim.Diagnoses;

namespace OrganismSim.Core
{
    public sealed class SeizureRiskRule : IRiskRule
    {
        public string Name => "Риск от гърч";
        public double ReferenceWindowSeconds => 60; // balance

        public double ComputeProbability(IReadOnlyDictionary<ParameterType, double> snapshot,
            PathologicalState pathology, Patient patient)
        {
            double brainDeficit = Deficit(snapshot, ParameterType.BrainFunction) / 100.0;
            double glucoseDeficit = Deficit(snapshot, ParameterType.BloodGlucoseLevel) / 50.0;
            double icpSeverity = pathology.GetSeverity(ConditionType.IntracranialProcess) / 10.0;

            double score = brainDeficit * 0.4 + glucoseDeficit * 0.3 + icpSeverity * 0.3; // balance
            return Math.Clamp(score, 0, 1);
        }

        public void Trigger(Patient patient, double score)
        {
            if (patient.Pathology.GetSeverity(ConditionType.Seizure) > 0) return;
            patient.Pathology.SetSeverity(ConditionType.Seizure, 3 + score * 5); // balance
            patient.Exposure.RecordSeizure();
        }

        private static double Deficit(IReadOnlyDictionary<ParameterType, double> snapshot, ParameterType type)
        {
            var range = ParameterRange.All[type];
            return Math.Max(0, range.NormalMin - snapshot[type]);
        }
    }

    public sealed class HypoglycemicCardioCerebralRiskRule : IRiskRule
    {
        public string Name => "Риск от инфаркт/инсулт при тежка хипогликемия";
        public double ReferenceWindowSeconds => 120; // balance

        public double ComputeProbability(IReadOnlyDictionary<ParameterType, double> snapshot,
            PathologicalState pathology, Patient patient)
        {
            double hypo = patient.Pathology.GetSeverity(ConditionType.Hypoglycemia);
            if (hypo < 6) return 0;

            double sympathetic = pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity);
            if (sympathetic < 6) return 0;

            return Math.Clamp((sympathetic - 6) / 4.0 * 0.15, 0, 0.15); // balance
        }

        public void Trigger(Patient patient, double score)
        {
            bool stroke = new Random().NextDouble() < 0.4; // balance
            if (stroke)
            {
                DiagnosisFactory.AddProfile(DiagnosisType.Stroke,
                    patient); //  I want to balance severity - трябва инсултът да е лек
                patient.Exposure.RecordStroke();
            }
            else
            {
                DiagnosisFactory.AddProfile(DiagnosisType.MyocardialInfarction,
                    patient); // I want to balance severity - трябва инфарктът да е лек
                patient.Exposure.RecordMyocardialInfraction();
            }
        }
    }

    public sealed class FallRiskRule : IRiskRule
    {
        public string Name => "Риск от падане";
        public double ReferenceWindowSeconds => 60; // balance

        public double ComputeProbability(IReadOnlyDictionary<ParameterType, double> snapshot,
            PathologicalState pathology, Patient patient)
        {
            if (patient.Posture != PatientPosture.Standing) return 0;
            double brainDeficit = Math.Max(0,
                                      ParameterRange.All[ParameterType.BrainFunction].NormalMin -
                                      snapshot[ParameterType.BrainFunction]) /
                                  80.0;
            return Math.Clamp(brainDeficit * 0.5, 0, 0.5); // balance
        }

        public void Trigger(Patient patient, double score)
        {
            patient.Posture = PatientPosture.Supine;
            DiagnosisFactory.AddProfile(DiagnosisType.Fall, patient); // I want to balance severity
            patient.Exposure.RecordFallings();
        }
    }

    public sealed class ChokingRiskRule : IRiskRule
    {
        public string Name => "Риск от задушаване";
        public double ReferenceWindowSeconds => 60; // balance

        public double ComputeProbability(IReadOnlyDictionary<ParameterType, double> snapshot,
            PathologicalState pathology, Patient patient)
        {
            if (patient.AirwayObstructionIsPositional) return 0;

            bool unconscious = snapshot[ParameterType.Consciousness] <= 2;
            bool supine = patient.Posture == PatientPosture.Supine;

            return unconscious && supine ? 0.3 : 0; // balance
        }

        public void Trigger(Patient patient, double _)
        {
            patient.Pathology.Adjust(ConditionType.AirwayObstruction, 5); // balance
            patient.AirwayObstructionIsPositional = true; // balance
            patient.Exposure.RecordChoking();
        }
    }


    public sealed class LethalArrhythmiaRiskRule : IRiskRule
    {
        public string Name => "Риск от животозастрашаваща аритмия";
        public double ReferenceWindowSeconds => 300; // balance

        public double ComputeProbability(IReadOnlyDictionary<ParameterType, double> snapshot,
            PathologicalState pathology, Patient patient)
        {
            double heartFailureSeverity = pathology.GetSeverity(ConditionType.HeartFailure);
            if (heartFailureSeverity <= 0) return 0;

            return Math.Clamp(heartFailureSeverity / 10.0 * 0.4, 0, 1); // balance
        }

        public void Trigger(Patient patient, double _)
        {
            patient.Physiology.Set(ParameterType.HeartRate, 210); // balance
            patient.Exposure.RecordLethalArrhythmia();
        }
    }
}
