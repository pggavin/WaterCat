using UnityEngine;

public static class GameSetup 
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void StartUp()
    {
        new GameObject("SoundSystem").AddComponent<SoundSystem>();
        // Make new obj with SoundSystem 
    }
}
