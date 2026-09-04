using System;
using System.Collections.Generic;

namespace OrganismSim.Core
{
    public class AbsorptionQueue
    {
        private sealed class Entry
        {
            public ParameterType Target;
            public double RemainingAmount;
            public double RatePerSecond;
        }

        private readonly List<Entry> _pending = new();

        public void Enqueue(ParameterType target, double totalAmount, double windowSeconds)
        {
            _pending.Add(new Entry
                { Target = target, RemainingAmount = totalAmount, RatePerSecond = totalAmount / windowSeconds });
        }

        public void ApplyTick(Patient patient, int seconds)
        {
            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                var entry = _pending[i];
                double deliver = Math.Min(entry.RemainingAmount, entry.RatePerSecond * seconds);
                entry.RemainingAmount -= deliver;
                patient.Physiology.Adjust(entry.Target, deliver);
                if (entry.RemainingAmount <= 0) _pending.RemoveAt(i);
            }
        }
    }
}