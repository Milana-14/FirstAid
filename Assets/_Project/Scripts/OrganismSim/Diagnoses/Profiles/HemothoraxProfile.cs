using System.Security.Cryptography;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class HemothoraxProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Hemothorax;
        public string DisplayName => "Хемоторакс";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.Hemothorax, 7);
        }
    }
}