namespace OrganismSim.Core
{
    public sealed class ConditionEngine
    {
        public void ApplyTick(Patient patient, int seconds)
        {
            foreach (var (conditionType, severity) in patient.Pathology.ActiveConditions())
            {
                ConditionEffectRegistry.Get(conditionType)?.ApplyTick(patient, seconds, severity);
            }
        }
    }
}