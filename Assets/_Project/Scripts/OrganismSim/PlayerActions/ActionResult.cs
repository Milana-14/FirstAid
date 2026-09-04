namespace OrganismSim.PlayerActions
{
    public readonly struct ActionResult
    {
        public ActionOutcome Outcome { get; }
        public string Message { get; }

        public ActionResult(ActionOutcome outcome, string message)
        {
            Outcome = outcome;
            Message = message;
        }
    }
}