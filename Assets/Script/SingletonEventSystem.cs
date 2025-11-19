using UnityEngine;
using UnityEngine.EventSystems;

// DITEMPEL DI OBJEK 'EventSystem' DI SCENE MAINMENU
[RequireComponent(typeof(EventSystem))]
public class SingletonEventSystem : MonoBehaviour
{
    public static SingletonEventSystem instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Jadikan abadi
        }
        else
        {
            // Jika sudah ada EventSystem lain (dari scene sebelumnya),
            // hancurkan yang ini.
            Destroy(gameObject);
        }
    }
}