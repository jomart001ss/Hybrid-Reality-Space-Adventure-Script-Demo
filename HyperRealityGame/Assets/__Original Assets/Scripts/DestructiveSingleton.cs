using UnityEngine;

/// <summary>
///A singleton that's made specifically for gameobjects.
///If another object spawns in the scene with the same class
///it'll be destroyed. base.Start () needs to be called in the
///child's Start () if it's used.
/// </summary>
public class DestructiveSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public void Awake ()
    {
        T type = instance; //initialize if nothing else calls it
        RemoveDuplicates();
    }

    public static DestructiveSingleton<T> instanceParent;
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance != null)
                {
                    DestructiveSingleton<T> destructiveSingleton = _instance as DestructiveSingleton<T>;
                    instanceParent = destructiveSingleton;
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    void RemoveDuplicates()
    {
        if (_instance != null && instanceParent != this)
        {
            Destroy(gameObject);
        }
    }
}