using UnityEngine;

public class InGameCanvasRefs : MonoBehaviour
{
    public GameObject hatOn;
    public GameObject hatOff;

    public GameObject staffOn;
    public GameObject staffOff;

    public GameObject bookOn;
    public GameObject bookOff;

    private void Start()
    {
       if(PlayerDataManager.Instance.GetComponent<PlayerData>().hatCollected)
            hatOn.SetActive(true);
       else
            hatOff.SetActive(true);

        if (PlayerDataManager.Instance.GetComponent<PlayerData>().staffCollected)
            staffOn.SetActive(true);
        else
            staffOff.SetActive(true);

        if (PlayerDataManager.Instance.GetComponent<PlayerData>().bookCollected)
            bookOn.SetActive(true);
        else
            bookOff.SetActive(true);
    }
}
