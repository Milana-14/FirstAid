using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class CreateParentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Create Empty Parent"))
        {
            CreateParent((Transform)target);
        }
    }

    private void CreateParent(Transform child)
    {
        // Запоминаем мировую трансформацию
        Vector3 worldPosition = child.position;
        Quaternion worldRotation = child.rotation;

        // Создаём Empty
        GameObject parent = new GameObject(child.name + "_Parent");

        // Ставим Parent на место объекта
        parent.transform.position = worldPosition;
        parent.transform.rotation = worldRotation;
        parent.transform.localScale = Vector3.one;

        // Делаем объект дочерним
        child.SetParent(parent.transform, false);

        // Сбрасываем локальные координаты
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;

        // Undo
        Undo.RegisterCreatedObjectUndo(parent, "Create Empty Parent");

        // Выбираем Parent
        Selection.activeGameObject = parent;
    }
}