using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text dialogText;
    public GameObject dialogPanel;
    public float typingSpeed = 0.03f;

    private List<string> messages = new List<string>();
    private int currentMessageIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialogPanel.SetActive(false); // Baþta kapalý kalsýn
    }

    void Update()
    {
        if (dialogPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            ShowNextMessage();
        }
    }

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
        if (currentMessageIndex < messages.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(messages[currentMessageIndex]));
            currentMessageIndex++;
        }
        else
        {
            dialogPanel.SetActive(false); // Bittiðinde panel kapanýr
        }
    }

    private IEnumerator TypeText(string textToDisplay)
    {
        dialogText.text = "";
        foreach (char c in textToDisplay)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}