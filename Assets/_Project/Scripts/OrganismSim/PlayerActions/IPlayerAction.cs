using OrganismSim.Core;

namespace OrganismSim.PlayerActions
{
    public interface IPlayerAction
    {
        string NameKey { get; }
        ActionResult Execute(Patient patient);
    }
}