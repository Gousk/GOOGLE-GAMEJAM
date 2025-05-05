using UnityEngine;

public class MapVisibilityManager : MonoBehaviour
{
    public GameObject TopFloor;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BottomFloor"))
        {
            TopFloor.SetActive(false);        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BottomFloor"))
        {
            TopFloor.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BottomFloor") && TopFloor != null)
        {
            TopFloor.SetActive(false);
        }
    }
}
