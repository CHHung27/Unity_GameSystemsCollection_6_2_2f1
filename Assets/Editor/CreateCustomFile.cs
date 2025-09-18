using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

/// <summary>
/// allows for users to create custom file types (txt, json, xml, csv) through Unity Asset Creation menu
/// </summary>
public static class CreateCustomFile
{
    // Base function that asks Unity to prompt for a name
    private static void CreateFile(string extension, string defaultName)
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
            path = "Assets";

        if (Directory.Exists(path) == false)
            path = Path.GetDirectoryName(path);

        string fullPath = Path.Combine(path, defaultName + extension);

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<DoCreateFile>(),
            fullPath,
            null,
            extension
        );
    }

    [MenuItem("Assets/Create/Text File", false, 80)]
    private static void CreateTextFile()
    {
        CreateFile(".txt", "NewTextFile");
    }

    [MenuItem("Assets/Create/JSON File", false, 81)]
    private static void CreateJsonFile()
    {
        CreateFile(".json", "NewJsonFile");
    }

    [MenuItem("Assets/Create/CSV File", false, 82)]
    private static void CreateCsvFile()
    {
        CreateFile(".csv", "NewCsvFile");
    }

    [MenuItem("Assets/Create/XML File", false, 83)]
    private static void CreateXmlFile()
    {
        CreateFile(".xml", "NewXmlFile");
    }

    // Handles the actual file creation
    private class DoCreateFile : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            File.WriteAllText(pathName, ""); // create an empty file
            AssetDatabase.ImportAsset(pathName);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }
}
