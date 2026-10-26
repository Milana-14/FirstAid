using OrganismSim.PlayerActions;

namespace _Project.Scripts.AI
{
    public readonly struct PlayerActionEntry
    {
        public ActionOutcome Outcome { get; }
        public string MessageKey { get; }
        public int Seconds { get; }
        public object[] MessageArgs { get; }

        public PlayerActionEntry(ActionOutcome outcome, string messageKey, int seconds, object[] messageArgs = null)
        {
            Outcome = outcome;
            MessageKey = messageKey;
            Seconds = seconds;
            MessageArgs = messageArgs;
        }
    }
}