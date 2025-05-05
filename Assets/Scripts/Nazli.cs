using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dialogText = null;
    [SerializeField] private GameObject dialogPanel = null;

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    private List<string> messages = new List<string>();
    private int currentMessageIndex = 0;
    private Coroutine typingCoroutine = null;
    private bool isTyping = false;

    void Awake()
    {
        // Hide at start
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogPanel.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Skip to full text
                StopCoroutine(typingCoroutine);
                dialogText.text = messages[currentMessageIndex];
                isTyping = false;
            }
            else
            {
                // Move to next message
                ShowNextMessage();
            }
        }
    }

    /// <summary>
    /// Kick off a new dialogue sequence.
    /// </summary>
    public void StartDialogue(List<string> newMessages)
    {
        if (newMessages == null || newMessages.Count == 0)
            return;

        messages = newMessages;
        currentMessageIndex = 0;
        dialogPanel.SetActive(true);
        ShowNextMessage();
    }

    private void ShowNextMessage()
    {
        if (currentMessageIndex >= messages.Count)
        {
            EndDialogue();
            return;
        }

        // Begin typing
        typingCoroutine = StartCoroutine(TypeText(messages[currentMessageIndex]));
        currentMessageIndex++;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (var c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        dialogPanel.SetActive(false);
        dialogText.text = "";
    }
}
