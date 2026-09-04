using System.Collections.Generic;

namespace OrganismSim.PlayerActions
{
    public static class ActionCatalog
    {
        public static List<IWaitPlayerAction> WaitBuildDefault()
        {
            return new List<IWaitPlayerAction>
            {
                new Wait1SecAction(),
                new Wait5SecAction(),
                new Wait10SecAction(),
                new Wait20SecAction(),
                new Wait1MinAction(),
            };
        }

        public static List<IPlayerAction> BuildDefault()
        {
            return new List<IPlayerAction>
            {
                new GiveWater(),
                new GiveJuice(),
                new GiveBiscuit(),
            };
        }
    }
}