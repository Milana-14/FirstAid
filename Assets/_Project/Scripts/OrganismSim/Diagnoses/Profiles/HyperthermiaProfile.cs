using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public sealed class HyperthermiaProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Hyperthermia;
        public string DisplayName => "Прегряване";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.Hyperthermia, 6);
        }
    }
}