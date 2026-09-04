using System;
using System.Security.Cryptography;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public class AnaphylaxisProfile : IDiagnosisProfile
    {
        public DiagnosisType Type => DiagnosisType.Anaphylaxis;
        public string DisplayName => "Анафилаксия";

        public void Initialize(Patient patient)
        {
            patient.Pathology.SetSeverity(ConditionType.Anaphylaxis, 7);
            
            var values = (AnaphylaxisPhenotype[])Enum.GetValues(typeof(AnaphylaxisPhenotype));
            patient.AnaphylaxisPhenotype = values[RandomNumberGenerator.GetInt32(values.Length)];
        }
    }
}