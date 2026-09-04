using System;
using System.Collections.Generic;

namespace OrganismSim.Core
{
    public sealed class PhysiologicalState
    {
        private readonly Dictionary<ParameterType, double> _values = new();

        public PhysiologicalState()
        {
            foreach (var p in ParameterRange.All)
            {
                _values[p.Key] = (p.Value.NormalMin + p.Value.NormalMax) / 2;
            }
        }

        public double Get(ParameterType parameterType) => _values[parameterType];

        public void Set(ParameterType parameterType, double value)
        {
            ParameterRange range = ParameterRange.All[parameterType];
            _values[parameterType] = Math.Clamp(value, range.Min, range.Max);
        }

        public void Adjust(ParameterType parameterType, double delta)
        {
            Set(parameterType, Get(parameterType) + delta);
        }

        public DeathCause GetDeathCause(ParameterType parameterType)
        {
            if (!DeathCauseMap.Causes.TryGetValue(parameterType, out var causes)) return DeathCause.None;

            ParameterRange range = ParameterRange.All[parameterType];
            double value = Get(parameterType);

            if (value <= range.LethalLow) return causes.Low;
            if (value >= range.LethalHigh) return causes.High;
            return DeathCause.None;
        }

        public Dictionary<ParameterType, double> SnapshotCopy() => new(_values);

        public void RecomputeDerived(PathologicalState pathology)
        {
            var snapshot = SnapshotCopy();
            foreach (var (type, rule) in DerivedParameterRegistry.All)
            {
                Set(type, rule.Compute(snapshot, pathology));
            }
        }

        public IReadOnlyDictionary<ParameterType, double> Snapshot() => _values;
    }
}