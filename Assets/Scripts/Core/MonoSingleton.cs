using UnityEngine;

// This script is monobehaviour scripts which act as singletons,
// so singleton scripts dont have to contain a bunch of repeated code

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                    Debug.LogError("No instance of " + typeof(T) + " found in the scene.");
            }

            return instance;
        }
    }
    // You can call the current copy of this script by doing ScriptName.Instance

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        instance = this as T;
        Initialize();
    }
    // We set this scripts instance to this object, deletes if there are duplicates

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
    // Sets null reference to script if this script is deleted and is our current instance
    // If you want a MonoSingleton to persist between scenes just use DontDestroyOnLoad

    protected virtual void Initialize()
    {
        // Code that would normally be in awake should be here
        // so use    protected override void Initialize()
    }
}
