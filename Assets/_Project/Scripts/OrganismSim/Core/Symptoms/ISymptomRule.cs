namespace OrganismSim.Core
{
    public interface ISymptomRule
    {
        SymptomType Type { get; }
        bool IsActive(Patient patient);
    }
}