using System.Collections.Generic;
using System.Data;
using System.Linq;
using OrganismSim.Core;

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
            new UnresponsiveRule(), ///
            new StuporRule(),
            new MuscleToneLossRule(),
            new LowBloodPressure(), ///
            new SlowBreathingRule(),
            new LeftArmWeaknessRule(),
            new PainRule(),
            new ShortnessOfBreathRule(),
        };

        public HashSet<SymptomType> Evaluate(Patient patient)
        {
            return _rules.Where(r => r.IsActive(patient)).Select(r => r.Type).ToHashSet();
        }
    }
}