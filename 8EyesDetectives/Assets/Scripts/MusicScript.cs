using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static MusicScript instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
