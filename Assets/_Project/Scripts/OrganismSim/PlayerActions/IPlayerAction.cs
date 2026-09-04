using OrganismSim.Core;

namespace OrganismSim.PlayerActions
{
    public interface IPlayerAction
    {
        string Name { get; }
        ActionResult Execute(Patient patient);
    }
}