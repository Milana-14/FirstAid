using System;
using System.Collections.Generic;

namespace OrganismSim.Core
{
    public sealed class RiskEngine
    {
        private readonly List<IRiskRule> _rules = new()
        {
            new SeizureRiskRule(),
            new HypoglycemicCardioCerebralRiskRule(),
            new FallRiskRule(),
            new ChokingRiskRule(),
            new LethalArrhythmiaRiskRule(),
        };

        private readonly Random _random = new();

        public void ApplyTick(Patient patient, IReadOnlyDictionary<ParameterType, double> snapshot, int seconds)
        {
            foreach (var rule in _rules)
            {
                double windowProbability = rule.ComputeProbability(snapshot, patient.Pathology, patient);
                if (windowProbability <= 0) continue;

                double tickProbability = 1 - Math.Pow(1 - windowProbability, seconds / rule.ReferenceWindowSeconds);

                if (_random.NextDouble() < tickProbability)
                {
                    rule.Trigger(patient, windowProbability);
                }
            }
        }
    }
}