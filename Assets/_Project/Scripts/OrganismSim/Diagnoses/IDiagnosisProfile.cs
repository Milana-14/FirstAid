using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public interface IDiagnosisProfile
    {
        DiagnosisType Type { get; }
        string DisplayName { get; }
        void Initialize(Patient patient);
    }
}