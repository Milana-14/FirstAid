using System.Collections.Generic;
using System.Linq;

namespace OrganismSim.Core
{
    public class ComplicationEngine
    {
        private readonly List<IComplicationRule> _rules = new()
        {
            new HypoxicBrainInjuryComplication(),
            new AspirationComplication(),
            new CardiacEventComplication(),
            new StrokeComplication(),
            new FallInjuryComplication(),
            new SurvivedArrhythmiaComplication(),
            new SeizureInjuryComplication(),
        };

        public IReadOnlyList<string> Evaluate(Patient patient) =>
            _rules.Where(r => r.Applies(patient)).Select(r => r.Description).ToList();
    }
}