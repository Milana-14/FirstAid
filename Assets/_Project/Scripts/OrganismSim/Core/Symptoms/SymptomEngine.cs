using System.Collections.Generic;
using System.Linq;

namespace OrganismSim.Core
{
    public sealed class SymptomEngine
    {
        private readonly List<ISymptomRule> _rules = new()
        {
            new HungerRule(),
            new SweatingRule(),
            new TremorRule(),
            new SalivationRule(),
            new PalpitationsRule(),
            new RapidWeakPulseRule(),
            new PanicRule(),
            new AnxietyRule(),
            new RestlessnessRule(),
            new AgitationRule(),
            new PalenessRule(),
            new MydriasisRule(),
            new NauseaRule(),
            new VomitingRule(),
            new WeaknessRule(),
            new HeadacheRule(),
            new DizzinessRule(),
            new DrowsinessRule(),
            new ParesthesiaRule(),
            new NumbnessAroundMouthRule(),
            new BlurredVisionRule(),
            new DoubleVisionRule(),
            new DifficultyConcentratingRule(),
            new ConfusionRule(),
            new DisorientationRule(),
            new SlurredSpeechRule(),
            new CoordinationProblemsRule(),
            new ApathyRule(),
            new AggressionRule(),
            new ReducedPainResponseRule(),
            new ReducedTouchResponseRule(),
            new ReducedTemperatureResponseRule(),
            new LossOfSwallowingReflexRule(),
            new LossOfSpeech(),
            new UnresponsiveRule(),
            new StuporRule(),
            new MuscleToneLossRule(),
            new LowBloodPressure(),
            new SlowBreathingRule(),
            new LeftArmWeaknessRule(),
            new PainRule(),
            new ShortnessOfBreathRule(),
        };

        private readonly SymptomSuppressionPolicy _suppressionPolicy = CreateDefaultPolicy();

        public List<SymptomType> Evaluate(Patient patient)
        {
            var rawActive = _rules
                .Where(r => r.IsActive(patient))
                .Select(r => r.Type)
                .ToList();

            return _suppressionPolicy.Apply(patient, rawActive);
        }

        private static SymptomSuppressionPolicy CreateDefaultPolicy()
        {
            var policy = new SymptomSuppressionPolicy();

            policy
                .SuppressWhenPresent(SymptomType.SlurredSpeech, SymptomType.LossOfSpeech, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Confusion, SymptomType.Disorientation, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Disorientation, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Drowsiness, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Stupor, SymptomType.Unresponsive)

                .SuppressWhenPresent(SymptomType.ReducedPainResponse, SymptomType.Unresponsive) ///
                .SuppressWhenPresent(SymptomType.ReducedTouchResponse, SymptomType.Unresponsive) ///
                .SuppressWhenPresent(SymptomType.ReducedTemperatureResponse, SymptomType.Unresponsive) ///

                .SuppressWhenPresent(SymptomType.Palpitations, SymptomType.RapidWeakPulse)
                .SuppressWhenPresent(SymptomType.BlurredVision, SymptomType.DoubleVision)

                .SuppressWhenPresent(SymptomType.Anxiety, SymptomType.Panic, SymptomType.Apathy)
                .SuppressWhenPresent(SymptomType.Panic, SymptomType.Apathy, SymptomType.Drowsiness, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Restlessness, SymptomType.Apathy, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Agitation, SymptomType.Apathy, SymptomType.Drowsiness, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Aggression, SymptomType.Apathy, SymptomType.Drowsiness, SymptomType.Stupor, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Apathy, SymptomType.Stupor, SymptomType.Unresponsive)

                .SuppressWhenPresent(SymptomType.CoordinationProblems, SymptomType.Stupor, SymptomType.MuscleToneLoss, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.LeftArmWeakness, SymptomType.MuscleToneLoss, SymptomType.Unresponsive)
                .SuppressWhenPresent(SymptomType.Tremor, SymptomType.MuscleToneLoss, SymptomType.Unresponsive);

            
           policy
                .DisableWhen(SymptomType.DifficultyConcentrating, p => p.Physiology.Get(ParameterType.Consciousness) < 25)
                .DisableWhen(SymptomType.Hunger, p => p.Physiology.Get(ParameterType.Consciousness) < 20)
                .DisableWhen(SymptomType.Paresthesia, p => p.Physiology.Get(ParameterType.Consciousness) < 17)
                .DisableWhen(SymptomType.Nausea, p => p.Physiology.Get(ParameterType.Consciousness) < 17)
                .DisableWhen(SymptomType.Headache, p => p.Physiology.Get(ParameterType.Consciousness) < 15)
                .DisableWhen(SymptomType.Dizziness, p => p.Physiology.Get(ParameterType.Consciousness) < 10)
                .DisableWhen(SymptomType.Weakness, p => p.Physiology.Get(ParameterType.Consciousness) < 10)
                .DisableWhen(SymptomType.NumbnessAroundMouth, p => p.Physiology.Get(ParameterType.Consciousness) < 10)
                .DisableWhen(SymptomType.BlurredVision, p => p.Physiology.Get(ParameterType.Consciousness) < 10)
                .DisableWhen(SymptomType.DoubleVision, p => p.Physiology.Get(ParameterType.Consciousness) < 10)
                
                .DisableWhen(SymptomType.Panic, p => p.Physiology.Get(ParameterType.Consciousness) < 20)
                .DisableWhen(SymptomType.Anxiety, p => p.Physiology.Get(ParameterType.Consciousness) < 20)
                .DisableWhen(SymptomType.Restlessness, p => p.Physiology.Get(ParameterType.Consciousness) < 20)
                .DisableWhen(SymptomType.Agitation, p => p.Physiology.Get(ParameterType.Consciousness) < 20)
                .DisableWhen(SymptomType.Aggression, p => p.Physiology.Get(ParameterType.Consciousness) < 20)
                
                .DisableWhen(SymptomType.Pain, p => p.Physiology.Get(ParameterType.Consciousness) <= 10)

                .DisableWhen(SymptomType.Salivation, p => p.Physiology.Get(ParameterType.Consciousness) < 15 || p.Pathology.GetSeverity(ConditionType.Shock) > 7);

            return policy;
        }
    }
}