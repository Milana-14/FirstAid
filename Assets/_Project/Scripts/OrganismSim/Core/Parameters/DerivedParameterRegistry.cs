using System.Collections.Generic;

namespace OrganismSim.Core
{
    public static class DerivedParameterRegistry
    {
        private static readonly Dictionary<ParameterType, IDerivedParameterRule> Rules = new()
        {
            [ParameterType.BloodPressure] = new BloodPressureRule(),
            [ParameterType.CerebralBloodFlow] = new CerebralBloodFlowRule(),
            [ParameterType.PeripheralOrganBloodFlow] = new PeripheralOrganBloodFlowRule(),
        };

        public static IReadOnlyDictionary<ParameterType, IDerivedParameterRule> All => Rules;
    }
}