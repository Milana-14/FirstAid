using System.Collections.Generic;
using System.Text;
using OrganismSim.Core;
using OrganismSim.PlayerActions;

namespace _Project.Scripts.AI
{
    public class ActionLog
    {
        private PatientController _patientController;
        
        private static List<PlayerActionEntry> _actionsLog = new();

        public ActionLog(PatientController patientController)
        {
            _patientController = patientController;

            if (_patientController != null)
                _patientController.OnLogAction += AddLogAction;
        }
        
        public void AddLogAction(ActionResult actionResult, int seconds)
        {
            _actionsLog.Add(new PlayerActionEntry(actionResult.Outcome, actionResult.MessageKey, seconds, actionResult.MessageArgs));
        }
        
        public string BuildPrompt(ScenarioOutcome scenarioOutcome, Patient patient, DeathCause deathCause, IReadOnlyList<string> complications)
        {
            string currentLang = "English";
            if (LocalizationService.Instance != null)
                currentLang = LocalizationService.Instance.GetCurrentLanguageName();

            StringBuilder actionHistory = new StringBuilder();
            
            if (_actionsLog.Count == 0)
                actionHistory.AppendLine("No actions were performed by the player.");
            else
                foreach (var log in _actionsLog)
                    actionHistory.AppendLine($"- At {log.Seconds}s | Outcome: {log.Outcome} | Action Key: {log.MessageKey}");

            string complicationsText = (complications != null && complications.Count > 0) 
                ? string.Join(", ", complications) 
                : "None";

            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.AppendLine("=== SYSTEM CONTEXT & ROLE ===");
            promptBuilder.AppendLine("You are an expert medical educator and evaluator analyzing a medical simulation scenario.");
            promptBuilder.AppendLine("Your goal is to evaluate the player's performance, explain what went right or wrong, and provide clear educational feedback.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("=== SCENARIO OUTCOME ===");
            promptBuilder.AppendLine($"Final Outcome: {scenarioOutcome}");
            promptBuilder.AppendLine($"Cause of Death: {(scenarioOutcome == ScenarioOutcome.Died ? deathCause.ToString() : "N/A")}");
            promptBuilder.AppendLine($"Complications Developed: {complicationsText}");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("=== CHRONOLOGICAL PLAYER ACTIONS ===");
            promptBuilder.Append(actionHistory.ToString());
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("=== INSTRUCTIONS FOR YOUR RESPONSE ===");
            promptBuilder.AppendLine("1. Provide a concise summary of the patient's condition and how the player handled it.");
            promptBuilder.AppendLine("2. Highlight critical mistakes (if any) and good clinical choices.");
            promptBuilder.AppendLine("3. Explain the pathophysiological consequences of the actions taken at specific timestamps.");
            promptBuilder.AppendLine("4. Keep the tone supportive, and educational, you can add some humor in your response if relevant.");
            promptBuilder.AppendLine("5. Your answer shouldn't be longer than 200 chars.");
            promptBuilder.AppendLine($"6. IMPORTANT: You MUST write your ENTIRE response in the following language: {currentLang}.");

            return promptBuilder.ToString();
        }
        
        public void Unsubscribe()
        {
            if (_patientController != null ) _patientController.OnLogAction -= AddLogAction;
        }
    }
}