using System;
using System.Collections.Generic;
using System.Linq;
using OrganismSim.Diagnoses;

namespace OrganismSim.Core
{
    public sealed class PathologicalState
    {
        private readonly Dictionary<ConditionType, double> _severities = new();

        public PathologicalState()
        {
            foreach (ConditionType type in Enum.GetValues(typeof(ConditionType)))
            {
                _severities[type] = 0;
            }
        }

        public double GetSeverity(ConditionType type) => _severities[type];

        public bool IsActive(ConditionType type) => _severities[type] > 0;

        public void SetSeverity(ConditionType type, double value) => _severities[type] = Math.Clamp(value, 0, 10);

        public void Adjust(ConditionType type, double delta) => SetSeverity(type, GetSeverity(type) + delta);

        public Dictionary<ConditionType, double> ActiveConditions() => _severities
            .Where(s => s.Value > 0)
            .ToDictionary(s => s.Key, s => s.Value);

        public IReadOnlyDictionary<ConditionType, double> Snapshot() => _severities;
    }
}