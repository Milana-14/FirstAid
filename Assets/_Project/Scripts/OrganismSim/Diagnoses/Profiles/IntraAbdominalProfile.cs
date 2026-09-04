using System.Security.Cryptography;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class IntraAbdominalBleedingProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.IntraAbdominalBleeding;
        public string DisplayName => "Вътрешно коремно кървене";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.IntraAbdominalBleeding, 7);
        }
    }
}