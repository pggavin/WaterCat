using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DebuggingData : MonoBehaviour
{
    [Header("Settings")]
    public int desiredFPS = 60;

    [Header("GUI Style")]
    [Space(15)]
    public int textSize = 16;
    public Font textFont;

    private GUIStyle _style;
    private float _fps;

    private void Start()
    {
        // Initialize GUI style
        _style = new GUIStyle();
        _style.font = textFont;
        _style.normal.textColor = Color.yellow;
        _style.fontSize = textSize;
        _style.alignment = TextAnchor.UpperLeft;
        _style.fontStyle = FontStyle.BoldAndItalic;

        // Lock frame rate to desiredFPS
        Application.targetFrameRate = desiredFPS;

        // Start coroutine to update FPS every second
        StartCoroutine(UpdateFPS());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            VisualEffects._score = 0;
    }

    private void OnGUI()
    {
        // Display FPS
        GUI.Label(new Rect(10, FPS_OFFSET,   100, 20), "FPS: "    + Mathf.Round(_fps), _style);
        GUI.Label(new Rect(10, TIME_OFFSET,  100, 20), "Time: "   + (int)Time.timeSinceLevelLoad, _style);
        GUI.Label(new Rect(10, SCORE_OFFSET, 100, 20), "Score: "  + VisualEffects._score, _style);
    }

    private IEnumerator<WaitForSeconds> UpdateFPS()
    {
        while (true)
        {
            _fps = 1 / Time.deltaTime;

            yield return new WaitForSeconds(1f);
        }
    }

    const int FPS_OFFSET = 10;
    const int TIME_OFFSET = 40;
    const int SCORE_OFFSET = 70;
}
// Shoutout to Raf he wrote the debug display script I just changed it a bit