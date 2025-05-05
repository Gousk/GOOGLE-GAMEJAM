using UnityEngine;

public class BoxDropPuzzle : MonoBehaviour
{
    public GameObject OpenOBJ;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BoxDrop"))
        {
            OpenOBJ.SetActive(true);
        }
    }
}
