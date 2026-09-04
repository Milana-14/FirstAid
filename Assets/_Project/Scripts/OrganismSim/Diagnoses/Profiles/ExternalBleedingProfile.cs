using System;
using System.Security.Cryptography;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class ExternalBleedingProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.ExternalBleeding;
        public string DisplayName => "Външно кървене";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.ExternalBleeding, 7);
            patient.Physiology.Adjust(ParameterType.Pain, 5);
            
            var values = (ExternalBleedingType[])Enum.GetValues(typeof(ExternalBleedingType));
            patient.ExternalBleedingType = values[RandomNumberGenerator.GetInt32(values.Length)];
        }
    }
}