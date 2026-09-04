using System.Collections.Generic;

namespace OrganismSim.Core
{
    public sealed class ParameterThresholdEngine
    {
        private readonly List<IParameterThresholdEffect> _effects = new()
        {
            new GasExchangeEffect(),
            new PulmonaryCapillaryPressureTrigger(),
            new HyperperfusionIcpEffect(),
            new HypoperfusionBrainDamageEffect(),
            new SystemicHypoperfusionEffect(),
            new ConsciousnessMetabolicEffect(),
            new RespiratoryDriveEffect(),
            new StressRegulationEffect(),
            new ComfortDecayEffect(),
            new AutonomicReflexEffect(),
            new OsmoticDehydrationTrigger(),
            new AirwayObstructionCapEffect(),
            new PositionalAirwayReliefEffect(),
        };

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            foreach (var effect in _effects)
            {
                effect.ApplyTick(patient, snapshot, seconds);
            }
        }
    }
}