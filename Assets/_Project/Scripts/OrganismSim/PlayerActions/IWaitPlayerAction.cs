namespace OrganismSim.PlayerActions
{
    public interface IWaitPlayerAction
    {
        string Name { get; }
        int TimeCostSeconds { get; }
    }
}