using System;
using System.Collections.Generic;
using OrganismSim.Core;

namespace OrganismSim.Core
{
    public sealed class HypoperfusionBrainDamageEffect : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double flowDeficit = Deficit(snapshot, ParameterType.CerebralBloodFlow);
            double oxygenDeficit = Deficit(snapshot, ParameterType.BloodOxygenLevel);

            double damage = seconds * (flowDeficit * 0.017 + oxygenDeficit * 0.03); // balance
            if (damage <= 0) return;

            patient.Physiology.Adjust(ParameterType.BrainFunction, -damage);
            patient.Physiology.Adjust(ParameterType.Consciousness, -damage);
        }

        private static double Deficit(IReadOnlyDictionary<ParameterType, double> snapshot, ParameterType type)
        {
            return Math.Max(0, ParameterRange.All[type].NormalMin - snapshot[type]);
        }
    }

    public sealed class SystemicHypoperfusionEffect : IParameterThresholdEffect
    {
        private const double MaxContribution = 40; // balance
        private const double ApproachRatePerSecond = 0.08; // balance

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double bpDeficitRatio = Math.Clamp(LowDeficitRatio(snapshot, ParameterType.BloodPressure), 0, 1);
            double floor = Math.Max(0,
                ParameterRange.All[ParameterType.Consciousness].NormalMax - MaxContribution * bpDeficitRatio);

            double current = snapshot[ParameterType.Consciousness];
            if (current <= floor) return;

            double delta = (current - floor) * ApproachRatePerSecond * seconds;
            patient.Physiology.Adjust(ParameterType.Consciousness, -delta);
            patient.Physiology.Adjust(ParameterType.BrainFunction, -delta * 0.7);
        }

        public double LowDeficitRatio(IReadOnlyDictionary<ParameterType, double> snapshot, ParameterType type)
        {
            var range = ParameterRange.All[type];
            double width = Math.Max(1, range.NormalMax - range.NormalMin);
            return Math.Max(0, (range.NormalMin - snapshot[type]) / width);
        }
    }

    public sealed class ConsciousnessMetabolicEffect : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double damage = GlucoseDamage(snapshot) + TemperatureDamage(snapshot);
            if (damage <= 0) return;

            double delta = seconds * damage;
            patient.Physiology.Adjust(ParameterType.BrainFunction, -delta * 0.7);
            patient.Physiology.Adjust(ParameterType.Consciousness, -delta);
        }

        private static double GlucoseDamage(IReadOnlyDictionary<ParameterType, double> snapshot)
        {
            var range = ParameterRange.All[ParameterType.BloodGlucoseLevel];
            double value = snapshot[ParameterType.BloodGlucoseLevel];

            const double neuroglycopenicFloor = 51.0; // balance
            if (value < neuroglycopenicFloor)
            {
                double deficit = neuroglycopenicFloor - value;
                return deficit * 0.03; // balance
            }

            if (value > range.NormalMax)
            {
                double excess = value - range.NormalMax;
                double lethalSpan = range.LethalHigh - range.NormalMax;
                double normalized = excess / lethalSpan;
                return normalized * normalized * 0.7; // balance — без изменений
            }

            return 0;
        }

        private static double TemperatureDamage(IReadOnlyDictionary<ParameterType, double> snapshot)
        {
            var range = ParameterRange.All[ParameterType.BodyTemperature];
            double value = snapshot[ParameterType.BodyTemperature];

            const double consciousFloorC = 32.0; // balance
            if (value < consciousFloorC)
            {
                double deficit = consciousFloorC - value;
                double span = consciousFloorC - range.LethalLow; // balance
                double normalized = Math.Clamp(deficit / span, 0, 1);
                return normalized * normalized * 0.9; // balance
            }


            if (value > range.NormalMax) return (value - range.NormalMax) * 0.3; // balance
            return 0;
        }
    }

    public sealed class RespiratoryDriveEffect : IParameterThresholdEffect
    {
        private const double SevereThreshold = 15; // balance

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double deficit = Math.Max(0, SevereThreshold - snapshot[ParameterType.BrainFunction]);
            if (deficit <= 0) return;

            patient.Physiology.Adjust(ParameterType.RespiratoryRate, seconds * -deficit * 0.1); // balance
        }
    }

    public sealed class GasExchangeEffect : IParameterThresholdEffect
    {
        private const double ApproachRatePerSecond = 0.03; // balance

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double targetCo2 = ComputeEquilibrium(patient, snapshot);
            double currentCo2 = snapshot[ParameterType.BloodCo2Level];

            patient.Physiology.Adjust(ParameterType.BloodCo2Level,
                (targetCo2 - currentCo2) * ApproachRatePerSecond * seconds);
        }

        private static double ComputeEquilibrium(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot)
        {
            const double referenceRate = 16; // balance
            const double referenceCo2 = 40; // balance
            const double minRate = 1;
            const double maxTargetCo2 = 100; // balance

            double lungInjury = patient.Pathology.GetSeverity(ConditionType.LungInjury);
            double pulmonaryEdema = patient.Pathology.GetSeverity(ConditionType.PulmonaryEdema);

            double pleuralBlood = snapshot[ParameterType.PleuralBloodVolume];
            ParameterRange pleuralRange = ParameterRange.All[ParameterType.PleuralBloodVolume];
            double pleuralRatio = pleuralBlood / Math.Max(1, pleuralRange.LethalHigh);

            double exchangeEfficiency = 1
                                        - lungInjury / 10.0 * 0.15 // balance
                                        - pulmonaryEdema / 10.0 * 0.1 // balance
                                        - pleuralRatio * 0.5; // balance

            exchangeEfficiency = Math.Clamp(exchangeEfficiency, 0.15, 1);

            double effectiveRate = Math.Max(minRate, snapshot[ParameterType.RespiratoryRate] * exchangeEfficiency);

            return Math.Min(maxTargetCo2, referenceCo2 * referenceRate / effectiveRate);
        }
    }

    public sealed class HyperperfusionIcpEffect : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            var range = ParameterRange.All[ParameterType.CerebralBloodFlow];
            double excess = Math.Max(0, snapshot[ParameterType.CerebralBloodFlow] - range.NormalMax);
            if (excess <= 0) return;

            patient.Physiology.Adjust(ParameterType.IntracranialPressure, seconds * excess * 0.05); // balance
        }
    }

    public sealed class StressRegulationEffect : IParameterThresholdEffect
    {
        private const double ApproachRatePerSecond = 0.05; // balance

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double target = ComputeTarget(patient, snapshot);
            double current = snapshot[ParameterType.Stress];

            patient.Physiology.Adjust(ParameterType.Stress, (target - current) * ApproachRatePerSecond * seconds);
        }

        private static double ComputeTarget(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot)
        {
            double target = 1 + PathologyLoad(patient);
            target += patient.Pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity) * 0.25;
            target -= patient.Pathology.GetSeverity(ConditionType.IncreasedParasympatheticActivity) * 0.25;
            target -= patient.Physiology.Get(ParameterType.Comfort) * 0.4;
            target += Math.Max(0, snapshot[ParameterType.Pain] - ParameterRange.All[ParameterType.Pain].NormalMax) *
                      0.3;
            return Math.Clamp(target, 0, 10);
        }

        private static readonly IReadOnlyDictionary<ConditionType, double> StressWeights =
            new Dictionary<ConditionType, double>
            {
                [ConditionType.ExternalBleeding] = 0.25, // видимата кръв - силен психологически тригер
                [ConditionType.Hemothorax] = 0.15, // вижда се кръв при дышане/кашляне — по-тревожно от скритата кръв
                [ConditionType.IntraAbdominalBleeding] = 0.1, // само слабост
                [ConditionType.PericardialTamponade] = 0.15, // тревога/беспокойство като ранен признак
                [ConditionType.IntracranialProcess] = 0.15,
                [ConditionType.AirwayObstruction] =
                    0.35, // "не мога да дишам" - най-паникьосващото усещане, което съществува
                [ConditionType.LungInjury] = 0.2,
                [ConditionType.PulmonaryEdema] = 0.25, // усещане за давене
                [ConditionType.HeartFailure] = 0.2,
                [ConditionType.ImpairedCerebralBloodFlow] = 0.1, // объркването намалява способността да усеща страх
                [ConditionType.Shock] = 0.2,
                [ConditionType.Hypoglycemia] = 0.15, // адренергичните симптоми са част от картината
                [ConditionType.Hyperglycemia] = 0.05, // бавно, малко остра тревога
                [ConditionType.Dehydration] = 0.05,
                [ConditionType.Seizure] = 0.1, // само по себе си самозатихва
                [ConditionType.Hypothermia] = 0.1,
                [ConditionType.Hyperthermia] = 0.1,
                [ConditionType.MildAllergicReaction] = 0.05,
                [ConditionType.Anaphylaxis] = 0.8, // вече обосновано с Pumphrey "impending doom"
                [ConditionType.Fracture111] = 0.05, // основното при фрактура минава през Pain, не тук — затова ниско
            };

        public static double PathologyLoad(Patient patient)
        {
            double load = 0;
            foreach (var (type, weight) in StressWeights)
                load += patient.Pathology.GetSeverity(type) * weight;
            return load;
        }
    }

    public sealed class ComfortDecayEffect : IParameterThresholdEffect
    {
        private const double DecayRate = 0.02;

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double load = StressRegulationEffect.PathologyLoad(patient);
            if (load <= 0) return;

            double current = snapshot[ParameterType.Comfort];
            if (current <= 0) return;

            double delta = load * DecayRate * seconds;
            patient.Physiology.Adjust(ParameterType.Comfort, -Math.Min(current, delta));
        }
    }

    public sealed class AutonomicReflexEffect : IParameterThresholdEffect
    {
        private const double ApproachRatePerSecond = 0.05; // balance

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double drive = 0; // положително: симпатикус, отрицателно: парасимпатикус

            drive += LowDeficitRatio(snapshot, ParameterType.BloodPressure) * 0.3; // баро рефлекс
            drive += LowDeficitRatio(snapshot, ParameterType.BloodOxygenLevel) * 0.3; // хемо рефлекс
            drive += LowDeficitRatio(snapshot, ParameterType.BloodGlucoseLevel) * 0.2; // адренергичен отговор
            drive += (snapshot[ParameterType.BodyTemperature] >= 30.0
                ? LowDeficitRatio(snapshot, ParameterType.BodyTemperature)
                : 0) * 0.15; // треперене до 30 градуса
            drive += HighExcessRatio(snapshot, ParameterType.Stress) * 0.05; // balance
            drive -= HighExcessRatio(snapshot, ParameterType.BloodPressure) * 0.3; // високо кръвно потиска симпатикус

            double targetSympathetic = Math.Clamp(drive, 0, 1) * 10; // balance
            double targetParasympathetic = Math.Clamp(-drive, 0, 1) * 10; // balance

            double currentSympathetic = patient.Pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity);
            double currentParasympathetic =
                patient.Pathology.GetSeverity(ConditionType.IncreasedParasympatheticActivity);

            patient.Pathology.Adjust(ConditionType.IncreasedSympatheticActivity,
                (targetSympathetic - currentSympathetic) * ApproachRatePerSecond * seconds);
            patient.Pathology.Adjust(ConditionType.IncreasedParasympatheticActivity,
                (targetParasympathetic - currentParasympathetic) * ApproachRatePerSecond * seconds);
        }

        private static double LowDeficitRatio(IReadOnlyDictionary<ParameterType, double> snapshot, ParameterType type)
        {
            var range = ParameterRange.All[type];
            double width = Math.Max(1, range.NormalMax - range.NormalMin);
            return Math.Max(0, (range.NormalMin - snapshot[type]) / width);
        }

        private static double HighExcessRatio(IReadOnlyDictionary<ParameterType, double> snapshot, ParameterType type)
        {
            var range = ParameterRange.All[type];
            double width = Math.Max(1, range.NormalMax - range.NormalMin);
            return Math.Max(0, (snapshot[type] - range.NormalMax) / width);
        }
    }

    public sealed class PulmonaryCapillaryPressureTrigger : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            var range = ParameterRange.All[ParameterType.PulmonaryCapillaryPressure];
            double excess = Math.Max(0, snapshot[ParameterType.PulmonaryCapillaryPressure] - range.NormalMax);
            if (excess <= 0) return;

            patient.Pathology.Adjust(ConditionType.PulmonaryEdema, seconds * excess * 0.05); // balance
        }
    }

    public sealed class OsmoticDehydrationTrigger : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            var glucoseRange = ParameterRange.All[ParameterType.BloodGlucoseLevel];
            var tempRange = ParameterRange.All[ParameterType.BodyTemperature];

            double glucoseExcess = Math.Max(0, snapshot[ParameterType.BloodGlucoseLevel] - glucoseRange.NormalMax);
            double tempExcess = Math.Max(0, snapshot[ParameterType.BodyTemperature] - tempRange.NormalMax);

            double delta = seconds * (glucoseExcess * 0.00002 + tempExcess * 0.002); // balance
            if (delta <= 0) return;

            patient.Pathology.Adjust(ConditionType.Dehydration, delta);
        }
    }

    public sealed class AirwayObstructionCapEffect : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            double severity = patient.Pathology.GetSeverity(ConditionType.AirwayObstruction);
            if (severity <= 0) return;

            var range = ParameterRange.All[ParameterType.RespiratoryRate];
            double maxAchievableRate = range.NormalMax * (1 - severity / 10.0);
            double current = patient.Physiology.Get(ParameterType.RespiratoryRate);

            if (current > maxAchievableRate)
            {
                patient.Physiology.Set(ParameterType.RespiratoryRate, maxAchievableRate);
            }
        }
    }

    public sealed class PositionalAirwayReliefEffect : IParameterThresholdEffect
    {
        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            if (!patient.AirwayObstructionIsPositional) return;
            if (patient.Posture == PatientPosture.Supine) return;

            patient.Pathology.SetSeverity(ConditionType.AirwayObstruction, 0);
            patient.AirwayObstructionIsPositional = false;
        }
    }
}