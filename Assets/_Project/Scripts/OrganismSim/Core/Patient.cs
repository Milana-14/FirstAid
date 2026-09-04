using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrganismSim.Core
{
    public class Patient
    {
        public string Name { get; }
        public PhysiologicalState Physiology { get; } = new();
        public PathologicalState Pathology { get; } = new();

        private readonly ConditionEngine _conditionEngine = new();
        private readonly ParameterThresholdEngine _parameterThresholdEngine = new();
        private readonly RiskEngine _riskEngine = new();
        private readonly SymptomEngine _symptomEngine = new();
        private readonly HomeostasisEngine _homeostasisEngine = new();
        public ExposureLog Exposure { get; } = new();
        public AbsorptionQueue Absorptions { get; } = new();
        public PatientPosture Posture { get; set; } = PatientPosture.Supine;
        public bool AirwayObstructionIsPositional { get; set; }

        public ExternalBleedingType? ExternalBleedingType { get; set; }
        public HeartFailureType? HeartFailureType { get; set; }
        public AnaphylaxisPhenotype? AnaphylaxisPhenotype { get; set; }

        public bool IsAlive { get; private set; } = true;
        public DeathCause CauseOfDeath { get; private set; } = DeathCause.None;
        public HashSet<SymptomType> ActiveSymptoms { get; private set; } = new();

        public Patient(string patientName)
        {
            Name = patientName;
        }

        public ConsciousnessLevel GetConsciousnessLevel()
        {
            var p = this.Physiology.Get(ParameterType.Consciousness);
            if (p > 60) return ConsciousnessLevel.Alert;
            if (p > 20) return ConsciousnessLevel.Confused;
            return ConsciousnessLevel.Unresponsive;
        }

        public (IReadOnlyCollection<SymptomType> New, IReadOnlyCollection<SymptomType> Resolved,
            IReadOnlyDictionary<ParameterType, double> Parameters, IReadOnlyDictionary<ConditionType, double> Conditions
            ) Tick(int seconds)
        {
            if (!IsAlive)
                return (Array.Empty<SymptomType>(), Array.Empty<SymptomType>(),
                    new ReadOnlyDictionary<ParameterType, double>(null),
                    new ReadOnlyDictionary<ConditionType, double>(null));

            var startOfTickSnapshot = Physiology.SnapshotCopy();

            _homeostasisEngine.ApplyTick(this, seconds);
            _conditionEngine.ApplyTick(this, seconds);
            _parameterThresholdEngine.ApplyTick(this, startOfTickSnapshot, seconds);
            _riskEngine.ApplyTick(this, startOfTickSnapshot, seconds);
            Exposure.ApplyTick(this, seconds);
            Absorptions.ApplyTick(this, seconds);
            Physiology.RecomputeDerived(Pathology);

            HashSet<SymptomType> old = ActiveSymptoms;
            ActiveSymptoms = _symptomEngine.Evaluate(this);

            var newSymptoms = ActiveSymptoms.Except(old).ToList();
            var resolvedSymptoms = old.Except(ActiveSymptoms).ToList();

            EvaluateDeath();

            return (newSymptoms, resolvedSymptoms, Physiology.Snapshot(), Pathology.ActiveConditions());
        }

        private void EvaluateDeath()
        {
            if (!IsAlive) return;

            foreach (ParameterType p in DeathCauseMap.Priority)
            {
                DeathCause cause = Physiology.GetDeathCause(p);
                if (cause != DeathCause.None)
                {
                    IsAlive = false;
                    CauseOfDeath = cause;
                    return;
                }
            }
        }

        public bool IsStabilized()
        {
            if (!IsAlive) return false;
            if (this.Pathology.ActiveConditions().Any(c => c.Value > 1)) return false;

            foreach (var p in this.Physiology.Snapshot())
                if (!(ParameterRange.All[p.Key].NormalMin <= Physiology.Get(p.Key)
                      && Physiology.Get(p.Key) <= ParameterRange.All[p.Key].NormalMax))
                    return false;

            return true;
        }
    }
}