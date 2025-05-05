using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform player;                  // Oyuncu referansý
    public float openDistance = 3f;           // Açýlma mesafesi
    public float openSpeed = 2f;              // Açýlma hýzý
    public Vector3 openEulerRotation;         // Açýk pozisyondaki rotasyon farký (örn: 0, 90, 0)

    private Quaternion closedRotation;        // Baþlangýç rotasyonu
    private Quaternion targetRotation;        // Hedef rotasyon
    private bool isOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;  // Baþlangýçta kapalý rotasyonu kaydet
        targetRotation = closedRotation;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= openDistance && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;

            if (isOpen)
                targetRotation = Quaternion.Euler(transform.eulerAngles + openEulerRotation);
            else
                targetRotation = closedRotation;
        }

        // Dönüþü yumuþat
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }
}
