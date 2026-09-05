using System.Collections.Generic;
using System.Linq;
using OrganismSim.Core;
using UnityEngine;
using TMPro;

public sealed class TEST_PatientStatsUI : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private TMP_Text parametersText;
    [SerializeField] private TMP_Text conditionsText;

    private void OnEnable()
    {
        if (patientController == null) return;
        patientController.OnTick += UpdateParameters;
        patientController.OnTick += UpdateConditions;
    }
    
    private void OnDisable()
    {
        if (patientController == null) return;
        patientController.OnTick -= UpdateParameters;
        patientController.OnTick -= UpdateConditions;
    }
    
    private void UpdateParameters(Patient patient)
    {
        List<string> parameters = new List<string>();

        foreach (var p in patient.Physiology.Snapshot())
        {
            // if (Parameters.TryGetValue(p.Key, out string label)) parameters.Add($"{label}: {p.Value:F2}");
            // else parameters.Add($"{p.Key}: {p.Value:F2}");
            
            parameters.Add($"{Parameters.GetValueOrDefault(p.Key)}: {p.Value:F2}");
        }
        
        parametersText.text = string.Join("\n", parameters);
    }
    
    private void UpdateConditions(Patient patient)
    {
        List<string> conditions = new List<string>();

        foreach (var c in patient.Pathology.ActiveConditions())
        {
            // if (Conditions.TryGetValue(c.Key, out string label)) conditions.Add($"{label}: {c.Value:F2}");
            // else conditions.Add($"{c.Key}: {c.Value:F2}");
            
            conditions.Add($"{Conditions.GetValueOrDefault(c.Key)}: {c.Value:F2}");
        }
        
        conditionsText.text = string.Join("\n", conditions);
    }
    
    private static readonly Dictionary<ParameterType, string> Parameters = new() // трябва да се премести другаде
    {
        [ParameterType.BloodVolume] = "Обем на кръвта",
        [ParameterType.BloodPressure] = "Кръвно налягане",
        [ParameterType.PeripheralVascularResistance] = "Периферно съдово съпротивление",
        [ParameterType.HeartRate] = "Сърдечна честота",
        [ParameterType.CerebralBloodFlow] = "Мозъчно кръвоснабдяване",
        [ParameterType.PeripheralOrganBloodFlow] = "Кръвоснабдяване на периферните органи",
        [ParameterType.IntracranialPressure] = "Вътречерепно налягане",
        [ParameterType.PleuralBloodVolume] = "Обем на кръв в плевралната кухина",
        [ParameterType.RespiratoryRate] = "Дихателна честота",
        [ParameterType.BloodOxygenLevel] = "Ниво на кислород в кръвта",
        [ParameterType.BloodCo2Level] = "Ниво на CO2 в кръвта",
        [ParameterType.PulmonaryCapillaryPressure] = "Налягане в белодробните капиляри",
        [ParameterType.BloodGlucoseLevel] = "Ниво на кръвната захар",
        [ParameterType.BodyTemperature] = "Телесна температура",
        [ParameterType.Consciousness] = "Съзнание",
        [ParameterType.BrainFunction] = "Мозъчна функция",
        [ParameterType.Stress] = "Стрес",
        [ParameterType.Pain] = "Болка",
        [ParameterType.Comfort] = "Комфорт",
    };
    
    private static readonly Dictionary<ConditionType, string> Conditions = new() // трябва да се премести другаде
    {
        [ConditionType.ExternalBleeding] = "Външно кървене",
        [ConditionType.Hemothorax] = "Хемоторакс",
        [ConditionType.IntraAbdominalBleeding] = "Вътрешно коремно кървене",
        [ConditionType.PericardialTamponade] = "Перикардна тампонада",
        [ConditionType.IntracranialProcess] = "Вътречерепен процес",
        [ConditionType.AirwayObstruction] = "Запушване на дихателните пътища",
        [ConditionType.LungInjury] = "Увреждане на белите дробове",
        [ConditionType.PulmonaryEdema] = "Белодробен оток",
        [ConditionType.HeartFailure] = "Сърдечна недостатъчност",
        [ConditionType.ImpairedCerebralBloodFlow] = "Нарушено мозъчно кръвоснабдяване",
        [ConditionType.Shock] = "Шок",
        [ConditionType.Hypoglycemia] = "Хипогликемия",
        [ConditionType.Hyperglycemia] = "Хипергликемия",
        [ConditionType.Dehydration] = "Обезводняване",
        [ConditionType.Seizure] = "Гърч",
        [ConditionType.IncreasedSympatheticActivity] = "Увеличена симпатикова активност",
        [ConditionType.IncreasedParasympatheticActivity] = "Увеличена парасимпатикова активност",
        [ConditionType.Hypothermia] = "Хипотермия",
        [ConditionType.Hyperthermia] = "Хипертермия",
        [ConditionType.MildAllergicReaction] = "Лека алергична реакция",
        [ConditionType.Anaphylaxis] = "Анафилаксия",
        [ConditionType.Fracture111] = "Счупване"
    };
}