using System;
using System.Collections.Generic;
using System.Linq;

namespace OrganismSim.Core
{
    public sealed class SymptomSuppressionPolicy
    {
        private readonly Dictionary<SymptomType, HashSet<SymptomType>> _suppressedBy = new();
        private readonly Dictionary<SymptomType, List<Func<Patient, bool>>> _disableConditions = new();

        public SymptomSuppressionPolicy SuppressWhenPresent(SymptomType target, params SymptomType[] dominantSymptoms)
        {
            if (!_suppressedBy.TryGetValue(target, out var set))
            {
                set = new HashSet<SymptomType>();
                _suppressedBy[target] = set;
            }

            foreach (var d in dominantSymptoms)
                set.Add(d);

            return this;
        }

        public SymptomSuppressionPolicy DisableWhen(SymptomType target, Func<Patient, bool> predicate)
        {
            if (!_disableConditions.TryGetValue(target, out var list))
            {
                list = new List<Func<Patient, bool>>();
                _disableConditions[target] = list;
            }

            list.Add(predicate);
            return this;
        }

        public List<SymptomType> Apply(Patient patient, IReadOnlyCollection<SymptomType> candidateSymptoms)
        {
            var activeSet = new HashSet<SymptomType>(candidateSymptoms);
            var result = new List<SymptomType>(candidateSymptoms.Count);

            foreach (var symptom in candidateSymptoms)
            {
                if (_suppressedBy.TryGetValue(symptom, out var dominants) &&
                    dominants.Any(activeSet.Contains))
                {
                    continue;
                }

                if (_disableConditions.TryGetValue(symptom, out var predicates) &&
                    predicates.Any(p => p(patient)))
                {
                    continue;
                }

                result.Add(symptom);
            }

            return result;
        }
    }
}