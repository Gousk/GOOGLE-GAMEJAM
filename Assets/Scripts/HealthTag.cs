using UnityEngine;
using UnityEngine.UI;

public class HealthTag : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerDataManager.Instance.GetComponent<PlayerHealth>().healthBar = GetComponent<Slider>();   
    }
}
