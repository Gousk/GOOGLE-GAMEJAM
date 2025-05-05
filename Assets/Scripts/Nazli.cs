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
        dialogPanel.SetActive(false); // Ba�ta kapal� kals�n
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
            dialogPanel.SetActive(false); // Bitti�inde panel kapan�r
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