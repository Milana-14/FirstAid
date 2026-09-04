using System;

namespace OrganismSim.Core
{
    public sealed class ExternalBleedingEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.ExternalBleeding;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            if (patient.ExternalBleedingType == ExternalBleedingType.Arterial)
            {
                double bpRatio = patient.Physiology.Get(ParameterType.BloodPressure) /
                                 ParameterRange.All[ParameterType.BloodPressure].NormalMin;
                double rate = -severity * 0.042 * 3.0 *
                              Math.Clamp(bpRatio, 0.3, 1.3); // balance — забавя се, но никога не спира сам
                patient.Physiology.Adjust(ParameterType.BloodVolume, seconds * rate);
            }
            else
            {
                patient.Physiology.Adjust(ParameterType.BloodVolume, seconds * -severity * 0.042); // balance
                if (severity < 4)
                    patient.Pathology.Adjust(ConditionType.ExternalBleeding,
                        -seconds * 0.007); // balance — бавно самосъсирване само при леки венозни
            }
        }
    }

    public sealed class HemothoraxEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Hemothorax;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BloodVolume, seconds * -severity * 0.25); // balance
            patient.Physiology.Adjust(ParameterType.PleuralBloodVolume, seconds * severity * 0.02); // balance
        }
    }

    public sealed class IntraAbdominalBleedingEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.IntraAbdominalBleeding;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BloodVolume, seconds * -severity * 0.3); // balance

            if (severity >= 7)
            {
                double maxAchievable = ParameterRange.All[ParameterType.RespiratoryRate].NormalMax *
                                       (1 - (severity - 7) / 10.0); // balance
                double current = patient.Physiology.Get(ParameterType.RespiratoryRate);
                if (current > maxAchievable)
                    patient.Physiology.Set(ParameterType.RespiratoryRate, seconds * maxAchievable);
            }
        }
    }

    public sealed class PericardialTamponadeEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.PericardialTamponade;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BloodVolume, seconds * -severity * 0.02); // balance
        }
    }

    public sealed class ImpairedCerebralBloodFlowEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.ImpairedCerebralBloodFlow;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            if (severity < 10)
            {
                patient.Pathology.Adjust(ConditionType.ImpairedCerebralBloodFlow, seconds * 0.0015); // balance
            }
        }
    }

    public sealed class IntracranialProcessEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.IntracranialProcess;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.IntracranialPressure, seconds * severity * 0.4); // balance
        }
    }

    public sealed class AirwayObstructionEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.AirwayObstruction;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.Stress, seconds * -severity * 0.2); // balance
        }
    }

    public sealed class LungInjuryEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.LungInjury;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BloodOxygenLevel, seconds * -severity * 0.15); // balance
            patient.Physiology.Adjust(ParameterType.BloodCo2Level, seconds * severity * 0.15); // balance
            patient.Physiology.Adjust(ParameterType.Pain, seconds * severity * 0.15); // balance
        }
    }

    public sealed class PulmonaryEdemaEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.PulmonaryEdema;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BloodOxygenLevel, seconds * -severity * 0.05); // balance
            patient.Physiology.Adjust(ParameterType.BloodCo2Level, seconds * severity * 0.05); // balance
        }
    }

    public sealed class HeartFailureEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.HeartFailure;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            if (severity < 10)
            {
                patient.Pathology.Adjust(ConditionType.HeartFailure, seconds * 0.008); // balance
            }

            patient.Physiology.Adjust(ParameterType.Pain, seconds * severity * 0.01); // balance

            if (patient.HeartFailureType == HeartFailureType.Left)
            {
                patient.Physiology.Adjust(ParameterType.PulmonaryCapillaryPressure,
                    seconds * severity * 0.5); // balance
            }
        }
    }

    public sealed class ShockEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Shock;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            var bpRange = ParameterRange.All[ParameterType.BloodPressure];
            bool decompensated =
                patient.Physiology.Get(ParameterType.BloodPressure) <= bpRange.LethalLow * 1.3; // balance

            double direction = decompensated ? -1 : 1;
            patient.Physiology.Adjust(ParameterType.HeartRate, seconds * direction * severity * 0.15); // balance
            patient.Physiology.Adjust(ParameterType.RespiratoryRate, seconds * direction * severity * 0.15); // balance
        }
    }

    public sealed class HypoglycemiaEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Hypoglycemia;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            if (severity < 8) patient.Pathology.Adjust(ConditionType.Hypoglycemia, seconds * 0.007); // balance
            patient.Physiology.Adjust(ParameterType.BloodGlucoseLevel, seconds * -severity * 0.05); // balance
        }
    }

    public sealed class HyperglycemiaEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Hyperglycemia;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            if (severity < 10)
            {
                patient.Pathology.Adjust(ConditionType.Hyperglycemia, seconds * 0.004); // balance
            }

            patient.Physiology.Adjust(ParameterType.BloodGlucoseLevel, seconds * severity * 0.52); // balance
        }
    }

    public sealed class SeizureEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Seizure;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.Consciousness, seconds * -severity * 0.3); // balance
            patient.Physiology.Adjust(ParameterType.BloodOxygenLevel, seconds * -severity * 0.05); // balance
            patient.Pathology.Adjust(ConditionType.Seizure, seconds * -0.3); // balance
        }
    }



    public sealed class MildAllergicReactionEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.MildAllergicReaction;
        private const double DecayPerSecond = 0.0025;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.Pain, seconds * severity * 0.005); // balance

            patient.Pathology.Adjust(ConditionType.MildAllergicReaction, seconds * -DecayPerSecond);
        }
    }

    public sealed class AnaphylaxisEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Anaphylaxis;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            var (airwayFactor, vascularFactor) =
                GetPhenotypeFactors(patient.AnaphylaxisPhenotype ?? AnaphylaxisPhenotype.Balanced);

            patient.Physiology.Adjust(ParameterType.PeripheralVascularResistance,
                seconds * -severity * 0.1 * vascularFactor); // balance
            patient.Pathology.Adjust(ConditionType.AirwayObstruction,
                seconds * severity * 0.0018 * airwayFactor); // balance
            patient.Pathology.Adjust(ConditionType.Anaphylaxis, seconds * 0.003); // balance
        }

        private static (double airway, double vascular) GetPhenotypeFactors(AnaphylaxisPhenotype phenotype) =>
            phenotype switch
            {
                AnaphylaxisPhenotype.RespiratoryDominant => (1.4, 0.6),
                AnaphylaxisPhenotype.CardiovascularDominant => (0.6, 1.2),
                _ => (1.0, 1.0)
            };
    }

    public sealed class HypothermiaEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Hypothermia;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BodyTemperature, seconds * -severity * 0.004); // balance

            double bodyTemp = patient.Physiology.Get(ParameterType.BodyTemperature);
            if (bodyTemp < 32.0)
            {
                double coldDeficit = 32.0 - bodyTemp;
                patient.Physiology.Adjust(ParameterType.HeartRate, -seconds * coldDeficit * 0.6); // balance
            }
        }
    }

    public sealed class HyperthermiaEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Hyperthermia;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BodyTemperature, seconds * severity * 0.1); // balance
        }
    }

    public sealed class DehydrationEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.Dehydration;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.BloodVolume, seconds * -severity * 0.051); // balance
        }
    }



    public sealed class IncreasedSympatheticActivityEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.IncreasedSympatheticActivity;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.HeartRate, seconds * severity * 0.25); // balance
            patient.Physiology.Adjust(ParameterType.PeripheralVascularResistance, seconds * severity * 0.25); // balance
            patient.Physiology.Adjust(ParameterType.RespiratoryRate, seconds * severity * 0.08); // balance
        }
    }

    public sealed class IncreasedParasympatheticActivityEffect : IConditionEffect
    {
        public ConditionType Type => ConditionType.IncreasedParasympatheticActivity;

        public void ApplyTick(Patient patient, int seconds, double severity)
        {
            patient.Physiology.Adjust(ParameterType.HeartRate, seconds * -severity * 0.25); // balance
            patient.Physiology.Adjust(ParameterType.PeripheralVascularResistance,
                seconds * -severity * 0.25); // balance
            patient.Physiology.Adjust(ParameterType.RespiratoryRate, seconds * -severity * 0.1); // balance
        }
    }
}