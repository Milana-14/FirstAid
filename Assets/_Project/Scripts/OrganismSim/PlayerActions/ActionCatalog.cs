using System.Collections.Generic;

namespace OrganismSim.PlayerActions
{
    public static class ActionCatalog
    {
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