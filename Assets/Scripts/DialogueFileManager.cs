using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central hub that stores and provides references to dialogue files.
/// DialogueManager can ask this for a file by name or index.
/// </summary>
public class DialogueFileManager : MonoBehaviour
{
    //[Header("Dialogue Library")]
    [Tooltip("List of dialogue files available in the game.")]
    public List<TextAsset> dialogueFiles = new List<TextAsset>();

    // Quick lookup dictionary
    private Dictionary<string, TextAsset> dialogueLookup = new Dictionary<string, TextAsset>();

    private void Awake()
    {
        // Populate dictionary for easy name-based lookup
        foreach (var file in dialogueFiles)
        {
            if (file != null && !dialogueLookup.ContainsKey(file.name))
            {
                dialogueLookup[file.name] = file;
                Debug.Log($"Found file: {file.name}");
            }
        }
    }

    /// <summary>
    /// Get a dialogue file by name (matching the asset name in Unity).
    /// </summary>
    public TextAsset GetDialogue(string fileName)
    {
        if (dialogueLookup.TryGetValue(fileName, out TextAsset file))
        {
            return file;
        }

        Debug.LogError($"DialogueFileManager: Dialogue file '{fileName}' not found!");
        return null;
    }

    /// <summary>
    /// Get a dialogue file by index (useful for levels, linear order).
    /// </summary>
    public TextAsset GetDialogue(int index)
    {
        if (index >= 0 && index < dialogueFiles.Count)
        {
            return dialogueFiles[index];
        }

        Debug.LogError($"DialogueFileManager: Index {index} out of range!");
        return null;
    }
}
