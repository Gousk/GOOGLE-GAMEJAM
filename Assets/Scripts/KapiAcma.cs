using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform player;                  // Oyuncu referans�
    public float openDistance = 3f;           // A��lma mesafesi
    public float openSpeed = 2f;              // A��lma h�z�
    public Vector3 openEulerRotation;         // A��k pozisyondaki rotasyon fark� (�rn: 0, 90, 0)

    private Quaternion closedRotation;        // Ba�lang�� rotasyonu
    private Quaternion targetRotation;        // Hedef rotasyon
    private bool isOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;  // Ba�lang��ta kapal� rotasyonu kaydet
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

        // D�n��� yumu�at
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }
}
