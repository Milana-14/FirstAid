namespace OrganismSim.Core
{
    public sealed class HungerRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Hunger;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 70;
    }

    public sealed class SweatingRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Sweating;

        public bool IsActive(Patient p) =>
            p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 60 || p.Pathology.GetSeverity(ConditionType.Shock) > 3;
    }

    public sealed class TremorRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Tremor;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 65;
    }

    public sealed class SalivationRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Salivation;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 65;
    }

    public sealed class PalpitationsRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Palpitations;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.HeartRate) > 110;
    }

    public sealed class RapidWeakPulseRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.RapidWeakPulse;

        public bool IsActive(Patient p) =>
            p.Physiology.Get(ParameterType.HeartRate) > 100 && p.Physiology.Get(ParameterType.BloodPressure) < 100;
    }

    public sealed class PanicRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Panic;

        public bool IsActive(Patient p) =>
            p.Physiology.Get(ParameterType.Pain) > 6 || p.Physiology.Get(ParameterType.BloodOxygenLevel) < 80;
    }

    public sealed class AnxietyRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Anxiety;

        public bool IsActive(Patient p) =>
            p.Physiology.Get(ParameterType.Stress) > ParameterRange.All[ParameterType.Stress].NormalMax;
    }

    public sealed class RestlessnessRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Restlessness;
        public bool IsActive(Patient p) => p.Pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity) > 3;
    }

    public sealed class AgitationRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Agitation;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BrainFunction) < 70 &&
                                           p.Physiology.Get(ParameterType.BrainFunction) >= 40;
    }

    public sealed class PalenessRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Paleness;

        public bool IsActive(Patient p) =>
            p.Physiology.Get(ParameterType.BloodVolume) < 85 || p.Pathology.GetSeverity(ConditionType.Shock) > 2;
    }

    public sealed class MydriasisRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Mydriasis;
        public bool IsActive(Patient p) => p.Pathology.GetSeverity(ConditionType.IncreasedSympatheticActivity) > 3;
    }

    public sealed class NauseaRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Nausea;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 55 ||
                                           p.Pathology.GetSeverity(ConditionType.Shock) > 2;
    }

    public sealed class VomitingRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Vomiting;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 48 ||
                                           p.Pathology.GetSeverity(ConditionType.Shock) > 5;
    }

    public sealed class WeaknessRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Weakness;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.PeripheralOrganBloodFlow) < 80 ||
                                           p.Physiology.Get(ParameterType.BrainFunction) < 70;
    }

    public sealed class HeadacheRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Headache;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 55 ||
                                           p.Physiology.Get(ParameterType.IntracranialPressure) > 20;
    }

    public sealed class DizzinessRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Dizziness;

        public bool IsActive(Patient p) =>
            p.Physiology.Get(ParameterType.BloodPressure) < 90 || p.Physiology.Get(ParameterType.BloodOxygenLevel) < 92;
    }

    public sealed class DrowsinessRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Drowsiness;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 50;
    }

    public sealed class ParesthesiaRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Paresthesia;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 51;
    }

    public sealed class NumbnessAroundMouthRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.NumbnessAroundMouth;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 50;
    }

    public sealed class BlurredVisionRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.BlurredVision;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 51 ||
                                           p.Physiology.Get(ParameterType.Consciousness) < 60;
    }

    public sealed class DoubleVisionRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.DoubleVision;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 45;
    }

    public sealed class DifficultyConcentratingRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.DifficultyConcentrating;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodGlucoseLevel) < 51 ||
                                           p.Physiology.Get(ParameterType.BrainFunction) < 70;
    }


    public sealed class ConfusionRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Confusion;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 70;
    }

    public sealed class DisorientationRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Disorientation;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 60;
    }

    public sealed class SlurredSpeechRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.SlurredSpeech;

        public bool IsActive(Patient p) =>
            p.Pathology.GetSeverity(ConditionType.ImpairedCerebralBloodFlow) > 2 ||
            p.Physiology.Get(ParameterType.Consciousness) < 40;
    }

    public sealed class CoordinationProblemsRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.CoordinationProblems;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BrainFunction) < 55;
    }

    public sealed class ApathyRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Apathy;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BrainFunction) < 40 &&
                                           p.Physiology.Get(ParameterType.Consciousness) > 15;
    }

    public sealed class AggressionRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Aggression;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 55 &&
                                           p.Physiology.Get(ParameterType.Consciousness) >= 15;
    }

    public sealed class ReducedPainResponseRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.ReducedPainResponse;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) <= 2;
    }

    public sealed class ReducedTouchResponseRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.ReducedTouchResponse;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) <= 2;
    }

    public sealed class ReducedTemperatureResponseRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.ReducedTemperatureResponse;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) <= 2;
    }

    public sealed class LossOfSwallowingReflexRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.LossOfSwallowingReflex;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 15;
    }

    public sealed class LossOfSpeech : ISymptomRule
    {
        public SymptomType Type => SymptomType.LossOfSpeech;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 15;
    }

    public sealed class UnresponsiveRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Unresponsive;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) <= 2;
    }

    public sealed class StuporRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Stupor;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 25 &&
                                           p.Physiology.Get(ParameterType.Consciousness) >= 15;
    }

    public sealed class MuscleToneLossRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.MuscleToneLoss;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Consciousness) < 10;
    }

    public sealed class LowBloodPressure : ISymptomRule
    {
        public SymptomType Type => SymptomType.LowBloodPressure;

        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodPressure) <
                                           ParameterRange.All[ParameterType.BloodPressure].NormalMin;
    }

    public sealed class SlowBreathingRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.SlowBreathing;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.RespiratoryRate) < 10;
    }

    public sealed class LeftArmWeaknessRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.LeftArmWeakness;
        public bool IsActive(Patient p) => p.Pathology.GetSeverity(ConditionType.ImpairedCerebralBloodFlow) > 3;
    }

    public sealed class PainRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.Pain;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.Pain) > 2;
    }

    public sealed class ShortnessOfBreathRule : ISymptomRule
    {
        public SymptomType Type => SymptomType.ShortnessOfBreath;
        public bool IsActive(Patient p) => p.Physiology.Get(ParameterType.BloodOxygenLevel) < 90;
    }
}