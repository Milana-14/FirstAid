namespace OrganismSim.Core
{
    public enum DeathCause
    {
        None,
        BloodLoss, // BloodVolume <= lethalLow
        CirculatoryCollapse, // BloodPressure <= lethalLow
        HypertensiveCrisis, // BloodPressure >= lethalHigh
        CardiacArrest, // HeartRate <= lethalLow
        LethalArrhythmia, // HeartRate >= lethalHigh
        CerebralIschemia, // CerebralBloodFlow <= lethalLow
        OrganFailure, // PeripheralOrganBloodFlow <= lethalLow
        IntracranialPressure, // IntracranialPressure >= lethalHigh
        TensionHemothorax, // PleuralBloodVolume >= lethalHigh
        RespiratoryArrest, // RespiratoryRate <= lethalLow
        RespiratoryFailure, // RespiratoryRate >= lethalHigh
        Hypoxia, // BloodOxygenLevel <= lethalLow
        HypocapnicShock, // BloodCo2Level <= lethalLow
        HypercapnicRespiratoryFailure, // BloodCo2Level >= lethalHigh
        HyperglycemicCrisis, // BloodGlucoseLevel >= lethalHigh
        Hypothermia, // BodyTemperature <= lethalLow
        Hyperthermia, // BodyTemperature >= lethalHigh
        BrainDeath // BrainFunction <= lethalLow
    }
}