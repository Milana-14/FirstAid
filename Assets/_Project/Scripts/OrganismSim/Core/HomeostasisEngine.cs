namespace OrganismSim.Core
{
    public sealed class HomeostasisEngine
    {
        public void ApplyTick(Patient patient, int seconds)
        {
            foreach (var parameterType in ParameterRange.All.Keys)
            {
                if (DerivedParameterRegistry.All.ContainsKey(parameterType)) continue;

                ParameterRange range = ParameterRange.All[parameterType];
                double value = patient.Physiology.Get(parameterType);

                double target;
                if (value < range.NormalMin) target = range.NormalMin;
                else if (value > range.NormalMax) target = range.NormalMax;
                else continue;

                double pull = (target - value) * RecoveryRateFor(parameterType) * seconds;
                patient.Physiology.Adjust(parameterType, pull);
            }
        }

        private static double RecoveryRateFor(ParameterType type)
        {
            if (type == ParameterType.IntracranialPressure) return 0.05; // balance
            if (type == ParameterType.BloodVolume) return 0.005; // balance
            if (type == ParameterType.BloodCo2Level) return 0.003; // balance
            if (type == ParameterType.BodyTemperature) return 0.002; // balance
            if (type == ParameterType.BloodGlucoseLevel) return 0.003; // balance
            if (type == ParameterType.Comfort) return 0; // balance
            return 0.01;
        }
    }
}