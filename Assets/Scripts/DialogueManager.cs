using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(DialogueFileManager))]
public class DialogueManager : MonoBehaviour
{
    private enum RevealMode { Characters, Words }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI textUI;             // create a canvas with a TextMeshProUGUI and drag reference here
    [SerializeField] private DialogueFileManager fileManager;    // should be auto generated with on the same object

    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueFileName;
    [SerializeField] private RevealMode revealMode = RevealMode.Characters;

    [Tooltip("Delay between each character in Character mode.")]
    [SerializeField] private float charDelay = 0.05f;

    [Tooltip("Delay between each word in Word mode.")]
    [SerializeField] private float wordDelay = 0.2f;

    [Header("Line Timing Settings")]
    [Tooltip("Minimum seconds a short line stays on screen.")]
    [SerializeField] private float minLineDelay = 1.0f;
    [Tooltip("Maximum seconds a long line stays on screen.")]
    [SerializeField] private float maxLineDelay = 3.5f;
    [Tooltip("Approximate max character count to consider 'long'.")]
    [SerializeField] private int maxCharCount = 40;

    private List<string> dialogueLines = new List<string>();

    /// <summary>
    /// ensures auto added DialodueFileManager is also auto referenced
    /// </summary>
    private void OnValidate()
    {
        if (fileManager == null)
            fileManager = GetComponent<DialogueFileManager>();
    }

    private void Start()
    {
        if (textUI == null)
        {
            Debug.LogError("DialogueManager: No TextMeshProUGUI assigned!");
            return;
        }

        // Ensure auto sizing is enabled
        textUI.enableAutoSizing = true;

        LoadDialogueFile();
        if (dialogueLines.Count > 0)
        {
            StartCoroutine(RunDialogue());
        }
    }

    private void LoadDialogueFile()
    {
        if (fileManager == null)
        {
            Debug.LogError("DialogueManager: No DialogueFileManager assigned!");
            return;
        }

        TextAsset dialogueFile = fileManager.GetDialogue(dialogueFileName);
        if (dialogueFile == null) return;

        string[] lines = dialogueFile.text.Split(
            new[] { '\n' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
        dialogueLines = new List<string>(lines);
    }

    private IEnumerator RunDialogue()
    {
        foreach (string line in dialogueLines)
        {
            switch (revealMode)
            {
                case RevealMode.Characters:
                    yield return StartCoroutine(RevealCharacters(line));
                    break;
                case RevealMode.Words:
                    yield return StartCoroutine(RevealWords(line));
                    break;
            }

            // Dynamic pause after line
            float lineDelay = CalculateLineDelay(line.Length);
            yield return new WaitForSeconds(lineDelay);
        }
    }

    private IEnumerator RevealCharacters(string line)
    {
        textUI.text = line;
        textUI.maxVisibleCharacters = 0;

        for (int i = 0; i <= line.Length; i++)
        {
            textUI.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charDelay);
        }
    }

    private IEnumerator RevealWords(string line)
    {
        // TMP needs full text loaded for formatting
        textUI.text = line;
        textUI.maxVisibleCharacters = 0;

        string[] words = line.Split(' ');
        int visibleCount = 0;

        foreach (string word in words)
        {
            // Reveal characters in the current word + a space
            visibleCount += word.Length + 1;
            textUI.maxVisibleCharacters = visibleCount;

            yield return new WaitForSeconds(wordDelay);
        }
    }

    private float CalculateLineDelay(int charCount)
    {
        float t = Mathf.Clamp01((float)charCount / maxCharCount);
        return Mathf.Lerp(minLineDelay, maxLineDelay, t);
    }
}