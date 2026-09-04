using System.Collections.Generic;

namespace OrganismSim.Core
{
    public interface IDerivedParameterRule
    {
        ParameterType Type { get; }
        double Compute(IReadOnlyDictionary<ParameterType, double> snapshot, PathologicalState pathology);
    }
}