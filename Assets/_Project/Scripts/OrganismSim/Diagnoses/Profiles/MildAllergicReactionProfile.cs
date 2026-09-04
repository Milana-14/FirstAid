using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class MildAllergicReactionProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.MildAllergicReaction;
        public string DisplayName => "Лека алергична реакция";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.MildAllergicReaction, 3);
        }
    }
}