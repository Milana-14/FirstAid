namespace OrganismSim.Core
{
    public interface IConditionEffect
    {
        ConditionType Type { get; }
        void ApplyTick(Patient patient, int seconds, double severity);
    }
}