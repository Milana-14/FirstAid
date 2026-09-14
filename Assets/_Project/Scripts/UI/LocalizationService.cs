using System;
using OrganismSim.Core;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocalizationService : MonoBehaviour
{
    public static LocalizationService Instance { get; private set; }

    [SerializeField] private string uiTableName = "UI_Table";
    [SerializeField] private string medicalTableName = "Medical_Table";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GetLocalizedString(string tableName, string entryKey, Action<string> callback)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, entryKey);
        op.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded) callback?.Invoke(handle.Result);
            else callback?.Invoke($"[{entryKey}]");
        };
    }

    public void GetLocalizedStringWithArgs(string tableName, string entryKey, object[] args, Action<string> callback)
    {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, entryKey, args);
        op.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded) callback?.Invoke(handle.Result);
            else callback?.Invoke($"[{entryKey}]");
        };
    }

    public void GetSymptomName(SymptomType symptom, Action<string> callback)
    {
        GetLocalizedString(medicalTableName, $"Symptom_{symptom}", callback);
    }

    public void GetParameterName(ParameterType param, Action<string> callback)
    {
        GetLocalizedString(medicalTableName, $"Param_{param}", callback);
    }

    public void GetConditionName(ConditionType condition, Action<string> callback)
    {
        GetLocalizedString(medicalTableName, $"Condition_{condition}", callback);
    }
}