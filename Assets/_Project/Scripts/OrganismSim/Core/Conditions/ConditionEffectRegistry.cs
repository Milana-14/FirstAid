using System.Collections.Generic;

namespace OrganismSim.Core
{
    public class ConditionEffectRegistry
    {
        private static readonly Dictionary<ConditionType, IConditionEffect> Effects = new()
        {
            [ConditionType.ExternalBleeding] = new ExternalBleedingEffect(),
            [ConditionType.Hemothorax] = new HemothoraxEffect(),
            [ConditionType.IntraAbdominalBleeding] = new IntraAbdominalBleedingEffect(),
            [ConditionType.PericardialTamponade] = new PericardialTamponadeEffect(),
            [ConditionType.ImpairedCerebralBloodFlow] = new ImpairedCerebralBloodFlowEffect(),
            [ConditionType.IntracranialProcess] = new IntracranialProcessEffect(),
            [ConditionType.AirwayObstruction] = new AirwayObstructionEffect(),
            [ConditionType.LungInjury] = new LungInjuryEffect(),
            [ConditionType.PulmonaryEdema] = new PulmonaryEdemaEffect(),
            [ConditionType.HeartFailure] = new HeartFailureEffect(),
            [ConditionType.Shock] = new ShockEffect(),
            [ConditionType.Hypoglycemia] = new HypoglycemiaEffect(),
            [ConditionType.Hyperglycemia] = new HyperglycemiaEffect(),
            [ConditionType.Dehydration] = new DehydrationEffect(),
            [ConditionType.Seizure] = new SeizureEffect(),
            [ConditionType.IncreasedSympatheticActivity] = new IncreasedSympatheticActivityEffect(),
            [ConditionType.IncreasedParasympatheticActivity] = new IncreasedParasympatheticActivityEffect(),
            [ConditionType.Hypothermia] = new HypothermiaEffect(),
            [ConditionType.Hyperthermia] = new HyperthermiaEffect(),
            [ConditionType.MildAllergicReaction] = new MildAllergicReactionEffect(),
            [ConditionType.Anaphylaxis] = new AnaphylaxisEffect()
        };

        public static IConditionEffect? Get(ConditionType conditionType) => Effects[conditionType];
    }
}