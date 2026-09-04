using System.Collections.Generic;
using OrganismSim.Core;
using UnityEngine;
using TMPro;

public sealed class PatientSymptomsUI : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private TMP_Text symptomsText;

    private void OnEnable()
    {
        patientController.OnTick += UpdateSymptoms;
    }
    
    private void OnDisable()
    {
        patientController.OnTick -= UpdateSymptoms;
    }
    
    private void UpdateSymptoms(Patient patient)
    {
        List<string> activeSymptoms = new List<string>();

        foreach (var a in patient.ActiveSymptoms)
            activeSymptoms.Add(Symptoms[a]);
        
        symptomsText.text = string.Join("\n", activeSymptoms);
    }
    
    private static readonly Dictionary<SymptomType, string> Symptoms = new() // после ще го преместя някъде другаде
{
    [SymptomType.Hunger] = "Глад",
    [SymptomType.Sweating] = "Изпотяване",
    [SymptomType.Tremor] = "Треперене",
    [SymptomType.Salivation] = "Усилено слюноотделяне",
    [SymptomType.Palpitations] = "Сърцебиене",
    [SymptomType.RapidWeakPulse] = "Учестен и слаб пулс",
    [SymptomType.Panic] = "Паника",
    [SymptomType.Anxiety] = "Тревожност",
    [SymptomType.Restlessness] = "Безпокойство",
    [SymptomType.Agitation] = "Възбуда",
    [SymptomType.Paleness] = "Бледост",
    [SymptomType.Mydriasis] = "Разширени зеници",
    [SymptomType.Nausea] = "Гадене",
    [SymptomType.Vomiting] = "Повръщане",
    [SymptomType.Weakness] = "Слабост",
    [SymptomType.Headache] = "Главоболие",
    [SymptomType.Dizziness] = "Замайване",
    [SymptomType.Drowsiness] = "Сънливост",
    [SymptomType.Paresthesia] = "Мравучкане",
    [SymptomType.NumbnessAroundMouth] = "Изтръпване около устата",
    [SymptomType.BlurredVision] = "Замъглено зрение",
    [SymptomType.DoubleVision] = "Двойно виждане",
    [SymptomType.DifficultyConcentrating] = "Нарушена концентрация",
    [SymptomType.Confusion] = "Обърканост",
    [SymptomType.Disorientation] = "Дезориентация",
    [SymptomType.SlurredSpeech] = "Неясен говор",
    [SymptomType.CoordinationProblems] = "Нарушена координация",
    [SymptomType.Apathy] = "Апатия",
    [SymptomType.Aggression] = "Агресивност",
    [SymptomType.ReducedPainResponse] = "Намалена реакция на болка",
    [SymptomType.ReducedTouchResponse] = "Намалена реакция на допир",
    [SymptomType.ReducedTemperatureResponse] = "Намалена реакция на температура",
    [SymptomType.LossOfSwallowingReflex] = "Изчезване на гълтателния рефлекс",
    [SymptomType.LossOfSpeech] = "Загуба на речта",
    [SymptomType.Unresponsive] = "Не реагира на външни стимули",
    [SymptomType.Stupor] = "Оцепенение",
    [SymptomType.MuscleToneLoss] = "Загуба на мускулен тонус",
    [SymptomType.LowBloodPressure] = "Ниско кръвно налягане",
    [SymptomType.SlowBreathing] = "Забавено дишане",
    [SymptomType.LeftArmWeakness] = "Не може да движи лявата ръка",
    [SymptomType.Pain] = "Болка",
    [SymptomType.ShortnessOfBreath] = "Задух"
};
}