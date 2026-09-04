using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public sealed class FallProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Fall;
        public string DisplayName => "Падане и счупване на китка";

        public void Initialize(Patient patient)
        {
            // patient.Pathology.SetSeverity(ConditionType.Fracture, 5);
            patient.Physiology.Adjust(ParameterType.Pain, 5);
        }
    }
}