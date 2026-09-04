namespace OrganismSim.Core
{
    public enum ParameterType
    {
        BloodVolume, // обем на кръвта (вътре в кръвоносните съдове)
        BloodPressure, // кръвно налягане
        PeripheralVascularResistance, // периферно съдово съпротивление
        HeartRate, // сърдечна честота
        CerebralBloodFlow, // мозъчно кръвоснабдяване
        PeripheralOrganBloodFlow, // кръвоснабдяване на периферните органи
        IntracranialPressure, // вътречерепно налягане
        PleuralBloodVolume, // обем на кръв в плевралната кухина (хемоторакс)
        RespiratoryRate, // дихателна честота
        BloodOxygenLevel, // ниво на кислород (в кръвта)
        BloodCo2Level, // ниво на CO2 (в кръвта)
        PulmonaryCapillaryPressure, // налягане в белодробните капиляри
        BloodGlucoseLevel, // ниво на кръвната захар
        BodyTemperature, // телесна температура
        Consciousness, // съзнание (0 - кома)
        BrainFunction, // мозъчна функция
        Stress, // стрес
        Pain, // болка
        Comfort, // комфорт
    }
}