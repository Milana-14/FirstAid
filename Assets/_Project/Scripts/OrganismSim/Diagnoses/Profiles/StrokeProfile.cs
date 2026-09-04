using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public sealed class StrokeProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Stroke;
        public string DisplayName => "Инсулт";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.ImpairedCerebralBloodFlow, 5);
        }
    }
}