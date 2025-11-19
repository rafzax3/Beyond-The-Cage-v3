using UnityEngine;

// Ditempel di 'Main Camera' di scene LORONG (Lorong_Normal, dll.)
public class CameraFollow : MonoBehaviour
{
    // GameManager akan mengisi 'target' ini secara otomatis
    [HideInInspector]
    public Transform target;

    [Header("Pengaturan Kamera")]
    [Tooltip("Kecepatan kamera mengikuti player")]
    [SerializeField] private float smoothSpeed = 0.125f;
    
    [Tooltip("Posisi kamera relatif terhadap player (JANGAN UBAH Z)")]
    // INI YANG ANDA CARI:
    // X=0 -> Player di tengah
    // X=5 -> Player di kiri layar
    // X=-5 -> Player di kanan layar
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);


    // Gunakan LateUpdate agar kamera bergerak SETELAH player bergerak
    void LateUpdate()
    {
        // Jika tidak ada target (misal: di menu), jangan lakukan apa-apa
        if (target == null)
        {
            return;
        }

        // Hitung posisi yang diinginkan
        Vector3 desiredPosition = target.position + offset;
        
        // Jaga agar Z-axis kamera tidak ikut berubah (tetap di -10 atau Z aslinya)
        // Ini penting jika offset Anda memiliki nilai Z selain -10
        desiredPosition.z = transform.position.z; 

        // Gerakkan kamera secara mulus
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}