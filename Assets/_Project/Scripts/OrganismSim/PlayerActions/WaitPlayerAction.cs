namespace OrganismSim.PlayerActions
{
    public sealed class Wait1SecAction : IWaitPlayerAction
    {
        public string Name => "Изчакай 1 секунда";
        public int TimeCostSeconds => 1;
    }

    public sealed class Wait5SecAction : IWaitPlayerAction
    {
        public string Name => "Изчакай 5 секунди";
        public int TimeCostSeconds => 5;
    }

    public sealed class Wait10SecAction : IWaitPlayerAction
    {
        public string Name => "Изчакай 10 секунди";
        public int TimeCostSeconds => 10;
    }

    public sealed class Wait20SecAction : IWaitPlayerAction
    {
        public string Name => "Изчакай 20 секунди";
        public int TimeCostSeconds => 20;
    }

    public sealed class Wait1MinAction : IWaitPlayerAction
    {
        public string Name => "Изчакай 1 минута";
        public int TimeCostSeconds => 60;
    }
}