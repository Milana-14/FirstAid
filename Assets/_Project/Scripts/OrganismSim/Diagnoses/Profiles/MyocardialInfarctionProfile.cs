using System;
using System.Security.Cryptography;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class MyocardialInfarctionProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.MyocardialInfarction;
        public string DisplayName => "Инфаркт";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.HeartFailure, 6);
            
            var values = (HeartFailureType[])Enum.GetValues(typeof(HeartFailureType));
            patient.HeartFailureType = values[RandomNumberGenerator.GetInt32(values.Length)];
        }
    }
}