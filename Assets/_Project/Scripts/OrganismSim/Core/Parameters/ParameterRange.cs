using System.Collections.Generic;

namespace OrganismSim.Core
{
    public sealed class ParameterRange
    {
        public double Min { get; }
        public double Max { get; }
        public double NormalMin { get; }
        public double NormalMax { get; }
        public double LethalLow { get; }
        public double LethalHigh { get; }

        public ParameterRange(double min, double max, double normalMin, double normalMax, double lethalLow,
            double lethalHigh)
        {
            Min = min;
            Max = max;
            NormalMin = normalMin;
            NormalMax = normalMax;
            LethalLow = lethalLow;
            LethalHigh = lethalHigh;
        }

        public static readonly Dictionary<ParameterType, ParameterRange> All = new()
        {
            // % от нормалния обем кръв
            [ParameterType.BloodVolume] = new ParameterRange(min: 0, max: 100, normalMin: 90, normalMax: 100,
                lethalLow: 40, lethalHigh: double.MaxValue),

            // кръвно налягане, mmHg
            [ParameterType.BloodPressure] = new ParameterRange(min: 0, max: 300, normalMin: 90, normalMax: 120,
                lethalLow: 50, lethalHigh: 220),

            // периферно съдово съпротивление, % от нормата (не пряко летален)
            [ParameterType.PeripheralVascularResistance] = new ParameterRange(min: 0, max: 300, normalMin: 80,
                normalMax: 120, lethalLow: double.MinValue, lethalHigh: double.MaxValue),

            // сърдечна честота, удари/мин
            [ParameterType.HeartRate] = new ParameterRange(min: 0, max: 250, normalMin: 60, normalMax: 100,
                lethalLow: 30, lethalHigh: 200),

            // мозъчно кръвоснабдяване, % от нормата
            [ParameterType.CerebralBloodFlow] = new ParameterRange(min: 0, max: 150, normalMin: 90, normalMax: 110,
                lethalLow: 25, lethalHigh: double.MaxValue),

            // кръвоснабдяване на периферните органи, % от нормата
            [ParameterType.PeripheralOrganBloodFlow] = new ParameterRange(min: 0, max: 150, normalMin: 90,
                normalMax: 110, lethalLow: 20, lethalHigh: double.MaxValue),

            // вътречерепно налягане, mmHg
            [ParameterType.IntracranialPressure] = new ParameterRange(min: 0, max: 100, normalMin: 5, normalMax: 15,
                lethalLow: double.MinValue, lethalHigh: 35),

            // обем на кръв в плевралната кухина (хемоторакс), ml
            [ParameterType.PleuralBloodVolume] = new ParameterRange(min: 0, max: 3000, normalMin: 0, normalMax: 0,
                lethalLow: double.MinValue, lethalHigh: 1500),

            // дихателна честота, вдишвания/мин
            [ParameterType.RespiratoryRate] = new ParameterRange(min: 0, max: 60, normalMin: 12, normalMax: 20,
                lethalLow: double.MinValue, lethalHigh: 60),

            // насищане на кръвта с кислород, SpO2 %
            [ParameterType.BloodOxygenLevel] = new ParameterRange(min: 0, max: 100, normalMin: 95, normalMax: 100,
                lethalLow: 60, lethalHigh: double.MaxValue),

            // парциално налягане на CO2 в кръвта, mmHg
            [ParameterType.BloodCo2Level] = new ParameterRange(min: 0, max: 150, normalMin: 35, normalMax: 45,
                lethalLow: 15, lethalHigh: 90),

            // белодробно капилярно налягане, mmHg
            [ParameterType.PulmonaryCapillaryPressure] = new ParameterRange(min: 0, max: 50, normalMin: 6,
                normalMax: 12, lethalLow: double.MinValue, lethalHigh: double.MaxValue),

            // кръвна захар, mg/dL
            [ParameterType.BloodGlucoseLevel] = new ParameterRange(min: 0, max: 600, normalMin: 70, normalMax: 100,
                lethalLow: 20, lethalHigh: 600),

            // телесна температура, градус целзий
            [ParameterType.BodyTemperature] = new ParameterRange(min: 20, max: 45, normalMin: 36.1, normalMax: 37.2,
                lethalLow: 28, lethalHigh: 42),

            // ниво на съзнание, условна скала 0-100 (не пряко летален, 0 - кома)
            [ParameterType.Consciousness] = new ParameterRange(min: 0, max: 100, normalMin: 80, normalMax: 100,
                lethalLow: double.MinValue, lethalHigh: double.MaxValue),

            // мозъчна функция, условна скала 0-100
            [ParameterType.BrainFunction] = new ParameterRange(min: 0, max: 100, normalMin: 80, normalMax: 100,
                lethalLow: 15, lethalHigh: double.MaxValue),

            // ниво на стрес, условна скала 0-10 (не пряко летален)
            [ParameterType.Stress] = new ParameterRange(min: 0, max: 10, normalMin: 0, normalMax: 2,
                lethalLow: double.MinValue, lethalHigh: double.MaxValue),

            // ниво на болка, условна скала 0-10 (не пряко летален)
            [ParameterType.Pain] = new ParameterRange(min: 0, max: 10, normalMin: 0, normalMax: 1,
                lethalLow: double.MinValue, lethalHigh: double.MaxValue),

            // комфорт
            [ParameterType.Comfort] = new ParameterRange(min: 0, max: 10, normalMin: 0, normalMax: 0,
                lethalLow: double.MinValue, lethalHigh: double.MaxValue),
        };
    }
}