#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;

public class ReplaceTMPFontWindow : EditorWindow
{
    private TMP_FontAsset oldFont;
    private TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Font")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceTMPFontWindow>("Replace TMP Font");
    }

    private void OnGUI()
    {
        oldFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Old Font (optional)", oldFont, typeof(TMP_FontAsset), false);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace in Open Scenes & Prefabs") && newFont != null)
        {
            ReplaceInScene();
            ReplaceInPrefabs();
            AssetDatabase.SaveAssets();
            Debug.Log("Замена шрифта завершена!");
        }
    }

    private void ReplaceInScene()
    {
        var texts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (var tmp in texts)
        {
            if (EditorUtility.IsPersistent(tmp.gameObject)) continue;
            if (oldFont == null || tmp.font == oldFont)
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newFont;
                EditorUtility.SetDirty(tmp);
            }
        }
    }

    private void ReplaceInPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            bool changed = false;

            foreach (var tmp in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (oldFont == null || tmp.font == oldFont)
                {
                    tmp.font = newFont;
                    changed = true;
                }
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
#endif