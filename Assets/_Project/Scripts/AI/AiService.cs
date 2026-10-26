using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using OrganismSim.Core;

namespace _Project.Scripts.AI
{
    public class AiService
    {
        private readonly ActionLog _actionLog;
        private readonly PhaseController _phaseController;
        
        private readonly string _token;
        private const string Endpoint = "https://models.inference.ai.azure.com/chat/completions";
        
        public event Action<string> OnAiAnalysisDone;

        public AiService(ActionLog actionLog, PhaseController phaseController)
        {
            _actionLog = actionLog;
            _phaseController = phaseController;
            
            if (_phaseController != null)
            {
                _phaseController.OnScenarioEnded += CallAI;
            }
            
            _token = Environment.GetEnvironmentVariable("FIRSTAID_AI_TOKEN");
            if (string.IsNullOrWhiteSpace(_token))
                throw new Exception("AI токенът не е зададен. FIRSTAID_AI_TOKEN");
        }

        public async void CallAI(ScenarioOutcome scenarioOutcome, Patient patient, DeathCause deathCause, IReadOnlyList<string> complications)
        {
            string prompt = _actionLog.BuildPrompt(scenarioOutcome, patient, deathCause, complications);

            var requestData = new ChatRequest
            {
                model = "gpt-4o",
                messages = new List<ChatMessageDto> { new ChatMessageDto { role = "user", content = prompt } },
                max_tokens = 60
            };

            string jsonBody = JsonUtility.ToJson(requestData);

            using (UnityWebRequest www = new UnityWebRequest(Endpoint, "POST"))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Bearer " + _token);

                var operation = www.SendWebRequest();

                while (!operation.isDone) await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    string errorMessage = $"Error generating AI evaluation: {www.error} | {www.downloadHandler.text}";
                    OnAiAnalysisDone?.Invoke(errorMessage);
                }
                else
                {
                    string jsonResponse = www.downloadHandler.text;
                    string resultText = ExtractContentFromResponse(jsonResponse);
                    OnAiAnalysisDone?.Invoke(resultText);
                }
            }
        }

        private string ExtractContentFromResponse(string jsonResponse)
        {
            try
            {
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(jsonResponse);
                if (response != null && response.choices != null && response.choices.Length > 0)
                {
                    return response.choices[0].message.content;
                }
            }
            catch (Exception ex)
            {
                return $"Parsing error: {ex.Message}";
            }
            return jsonResponse;
        }
        
        public void Unsubscribe()
        {
            if (_phaseController != null) _phaseController.OnScenarioEnded -= CallAI;
        }
        
        
        [Serializable]
        private class ChatRequest
        {
            public string model;
            public List<ChatMessageDto> messages;
            public int max_tokens;
        }

        [Serializable]
        private class ChatMessageDto
        {
            public string role;
            public string content;
        }

        [Serializable]
        private class ChatResponse
        {
            public Choice[] choices;
        }

        [Serializable]
        private class Choice
        {
            public MessageDto message;
        }

        [Serializable]
        private class MessageDto
        {
            public string content;
        }
    }
}