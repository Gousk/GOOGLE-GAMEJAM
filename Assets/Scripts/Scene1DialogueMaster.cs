using UnityEngine;

public class Scene1DialogueMaster : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();

        dialogueManager.StartDialogue(0);
    }
}
