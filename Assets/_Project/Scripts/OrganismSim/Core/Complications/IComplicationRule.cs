namespace OrganismSim.Core
{
    public interface IComplicationRule
    {
        string Description { get; }
        bool Applies(Patient patient);
    }
}