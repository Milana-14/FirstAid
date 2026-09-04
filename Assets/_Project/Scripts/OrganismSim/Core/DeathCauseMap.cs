using System.Collections.Generic;

namespace OrganismSim.Core
{
    public static class DeathCauseMap
    {
        public static readonly Dictionary<ParameterType, (DeathCause Low, DeathCause High)> Causes = new()
        {
            [ParameterType.BloodVolume] = (DeathCause.BloodLoss, DeathCause.None),
            [ParameterType.BloodPressure] = (DeathCause.CirculatoryCollapse, DeathCause.HypertensiveCrisis),
            [ParameterType.HeartRate] = (DeathCause.CardiacArrest, DeathCause.LethalArrhythmia),
            [ParameterType.CerebralBloodFlow] = (DeathCause.CerebralIschemia, DeathCause.None),
            [ParameterType.PeripheralOrganBloodFlow] = (DeathCause.OrganFailure, DeathCause.None),
            [ParameterType.IntracranialPressure] = (DeathCause.None, DeathCause.IntracranialPressure),
            [ParameterType.PleuralBloodVolume] = (DeathCause.None, DeathCause.TensionHemothorax),
            [ParameterType.RespiratoryRate] = (DeathCause.RespiratoryArrest, DeathCause.RespiratoryFailure),
            [ParameterType.BloodOxygenLevel] = (DeathCause.Hypoxia, DeathCause.None),
            [ParameterType.BloodCo2Level] = (DeathCause.HypocapnicShock, DeathCause.HypercapnicRespiratoryFailure),
            [ParameterType.BloodGlucoseLevel] = (DeathCause.None, DeathCause.HyperglycemicCrisis),
            [ParameterType.BodyTemperature] = (DeathCause.Hypothermia, DeathCause.Hyperthermia),
            [ParameterType.BrainFunction] = (DeathCause.BrainDeath, DeathCause.None),
        };

        public static readonly IReadOnlyList<ParameterType> Priority = new List<ParameterType>
        {
            ParameterType.BloodOxygenLevel,
            ParameterType.RespiratoryRate,
            ParameterType.BloodCo2Level,
            ParameterType.HeartRate,
            ParameterType.BloodPressure,
            ParameterType.BloodVolume,
            ParameterType.CerebralBloodFlow,
            ParameterType.IntracranialPressure,
            ParameterType.PleuralBloodVolume,
            ParameterType.PeripheralOrganBloodFlow,
            ParameterType.BrainFunction,
            ParameterType.BloodGlucoseLevel,
            ParameterType.BodyTemperature,
        };
    }
}