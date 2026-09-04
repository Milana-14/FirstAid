using System.Security.Cryptography;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class PericardialTamponadeProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.PericardialTamponade;
        public string DisplayName => "Перикардна тампонада";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.PericardialTamponade, 7);
        }
    }
}