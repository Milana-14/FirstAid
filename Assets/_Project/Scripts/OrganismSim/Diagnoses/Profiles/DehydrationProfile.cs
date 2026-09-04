using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public sealed class DehydrationProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Dehydration;
        public string DisplayName => "Обезводняване";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.Dehydration, 6);
        }
    }
}