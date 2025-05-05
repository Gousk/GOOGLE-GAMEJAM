using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public enum SpeakerType { Book, Staff, Hat, Mage }

    [Serializable]
    public class DialogueLine
    {
        public SpeakerType speaker;
        [TextArea] public string message;
    }

    [Serializable]
    public class DialogueList
    {
        [Tooltip("Inspector reference name")]
        public string name;
        public List<DialogueLine> lines = new List<DialogueLine>();
    }

    [Header("UI References")]
    [SerializeField] private GameObject bookPanel = null;
    [SerializeField] private TMP_Text bookText = null;
    [SerializeField] private GameObject staffPanel = null;
    [SerializeField] private TMP_Text staffText = null;
    [SerializeField] private GameObject hatPanel = null;
    [SerializeField] private TMP_Text hatText = null;
    [SerializeField] private GameObject magePanel = null;
    [SerializeField] private TMP_Text mageText = null;

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("All Dialogue Lists")]
    [Tooltip("Add multiple DialogueLists; call by index.")]
    public List<DialogueList> dialogueLists = new List<DialogueList>();

    // States
    private enum DialogueState { Idle, Typing, Paused }
    private DialogueState state = DialogueState.Idle;

    // Current dialogue data
    private List<DialogueLine> currentMessages = null;
    private int currentIndex = -1;
    private Coroutine typingCoroutine = null;

    void Awake()
    {
        HideAllPanels();
    }

    void Update()
    {
        if (state == DialogueState.Idle)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (state == DialogueState.Typing)
            {
                // Skip typing
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);
                FinishTyping();
            }
            else if (state == DialogueState.Paused)
            {
                // Advance to next line
                ShowNextLine();
            }
        }
    }

    /// <summary>
    /// Call to start a dialogue by index.
    /// </summary>
    public void StartDialogue(int listIndex)
    {
        if (state != DialogueState.Idle)
            return;

        if (listIndex < 0 || listIndex >= dialogueLists.Count)
        {
            Debug.LogWarning($"DialogueManager: Invalid list index {listIndex}.");
            return;
        }

        var list = dialogueLists[listIndex];
        if (list.lines == null || list.lines.Count == 0)
        {
            Debug.LogWarning($"DialogueManager: Dialogue '{list.name}' has no lines.");
            return;
        }

        // Prepare
        StopAllCoroutines();
        HideAllPanels();
        currentMessages = list.lines;
        currentIndex = -1;

        // Hook
        OnDialogueStart();

        // Start
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        currentIndex++;
        Debug.Log(currentIndex + " " + currentMessages.Count+" TEST1");
        if (currentIndex >= currentMessages.Count)
        {
            Debug.Log(currentIndex + " " + currentMessages.Count + " TEST1");
            EndDialogue();
            return;
        }

        HideAllPanels();
        var line = currentMessages[currentIndex];
        var (panel, textField) = GetUI(line.speaker);
        if (panel == null || textField == null)
        {
            Debug.LogError($"DialogueManager: UI not assigned for speaker {line.speaker}.");
            return;
        }

        panel.SetActive(true);
        textField.text = string.Empty;
        state = DialogueState.Typing;
        typingCoroutine = StartCoroutine(TypeRoutine(textField, line.message));
    }

    private IEnumerator TypeRoutine(TMP_Text textField, string message)
    {
        foreach (char c in message)
        {
            textField.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        FinishTyping();
    }

    private void FinishTyping()
    {
        // Fill full text
        var line = currentMessages[currentIndex];
        var (_, textField) = GetUI(line.speaker);
        if (textField != null)
            textField.text = line.message;

        state = DialogueState.Paused;
    }

    private void EndDialogue()
    {
        HideAllPanels();
        state = DialogueState.Idle;
        currentMessages = null;

        // Hook
        OnDialogueEnd();
    }

    private (GameObject, TMP_Text) GetUI(SpeakerType speaker)
    {
        return speaker switch
        {
            SpeakerType.Book => (bookPanel, bookText),
            SpeakerType.Staff => (staffPanel, staffText),
            SpeakerType.Hat => (hatPanel, hatText),
            SpeakerType.Mage => (magePanel, mageText),
            _ => (null, null)
        };
    }

    private void HideAllPanels()
    {
        bookPanel?.SetActive(false);
        staffPanel?.SetActive(false);
        hatPanel?.SetActive(false);
        magePanel?.SetActive(false);
    }

    /// <summary>
    /// Override to react when dialogue starts.
    /// </summary>
    protected virtual void OnDialogueStart() { }

    /// <summary>
    /// Override to react when dialogue ends.
    /// </summary>
    protected virtual void OnDialogueEnd() { }
}
