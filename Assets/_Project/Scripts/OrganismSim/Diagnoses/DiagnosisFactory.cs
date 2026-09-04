using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OrganismSim.Core;

namespace OrganismSim.Diagnoses
{
    public static class DiagnosisFactory
    {
        private static readonly Dictionary<DiagnosisType, IDiagnosisProfile> Profiles = new()
        {
            [DiagnosisType.ExternalBleeding] = new ExternalBleedingProfile(),
            [DiagnosisType.Hemothorax] = new HemothoraxProfile(),
            [DiagnosisType.IntraAbdominalBleeding] = new IntraAbdominalBleedingProfile(),
            [DiagnosisType.PericardialTamponade] = new PericardialTamponadeProfile(),
            [DiagnosisType.MyocardialInfarction] = new MyocardialInfarctionProfile(),
            [DiagnosisType.Stroke] = new StrokeProfile(),
            [DiagnosisType.Hypoglycemia] = new HypoglycemiaProfile(),
            [DiagnosisType.Hyperglycemia] = new HyperglycemiaProfile(),
            [DiagnosisType.Fall] = new FallProfile(),
            [DiagnosisType.Anaphylaxis] = new AnaphylaxisProfile(),
            [DiagnosisType.MildAllergicReaction] = new MildAllergicReactionProfile(),
            [DiagnosisType.Hypothermia] = new HypothermiaProfile(),
            [DiagnosisType.Hyperthermia] = new MildAllergicReactionProfile(),
            [DiagnosisType.Dehydration] = new DehydrationProfile(),
        };

        public static IDiagnosisProfile GetProfile(DiagnosisType type) => Profiles[type];
        public static IReadOnlyList<IDiagnosisProfile> AllProfiles() => Profiles.Values.ToList();

        public static Patient Create(DiagnosisType type, string patientName)
        {
            Patient patient = new Patient(patientName);
            Profiles[type].Initialize(patient);
            return patient;
        }

        public static void AddProfile(DiagnosisType type, Patient patient)
        {
            Profiles[type].Initialize(patient);
        }
    }
}