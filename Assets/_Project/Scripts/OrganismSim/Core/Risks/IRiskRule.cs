using System.Collections.Generic;

namespace OrganismSim.Core
{
    public interface IRiskRule
    {
        string Name { get; }
        double ReferenceWindowSeconds { get; }

        double ComputeProbability(IReadOnlyDictionary<ParameterType, double> snapshot, PathologicalState pathology,
            Patient patient);

        void Trigger(Patient patient, double score);
    }
}