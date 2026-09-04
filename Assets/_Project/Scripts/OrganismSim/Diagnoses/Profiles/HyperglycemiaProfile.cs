using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public sealed class HyperglycemiaProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Hyperglycemia;
        public string DisplayName => "Хипергликемия";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.Hyperglycemia, 6);
        }
    }
}