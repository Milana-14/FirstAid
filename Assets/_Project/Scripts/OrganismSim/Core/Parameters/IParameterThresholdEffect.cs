using System.Collections.Generic;

namespace OrganismSim.Core
{
    public interface IParameterThresholdEffect
    {
        void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds);
    }
}