namespace OrganismSim.PlayerActions
{
    public readonly struct ActionResult
    {
        public ActionOutcome Outcome { get; }
        public string MessageKey { get; }
        public object[] MessageArgs { get; }

        public ActionResult(ActionOutcome outcome, string messageKey, object[] messageArgs = null)
        {
            Outcome = outcome;
            MessageKey = messageKey;
            MessageArgs = messageArgs;
        }
    }
}