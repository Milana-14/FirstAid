using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public sealed class HypothermiaProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Hypothermia;
        public string DisplayName => "Преохлаждане";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.Hypothermia, 6);
        }
    }
}