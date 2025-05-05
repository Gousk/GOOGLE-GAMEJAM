using UnityEngine;

public class Scene2DialogueMaster : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();

        dialogueManager.StartDialogue(0);
    }

    public void BookDialogue()
    {
        dialogueManager.StartDialogue(1);
    }
}
