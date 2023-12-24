using UnityEngine;
using System.Collections;
using static UnityEngine.SceneManagement.SceneManager;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SceneTransitions : MonoSingleton<SceneTransitions>
{
    private Image _screenWipe;

    private void Start()
    {
        _screenWipe = GetComponent<Image>();
        // These values are set to their defaults just in case.
        _screenWipe.fillAmount = 0;
        _screenWipe.fillOrigin = 1;
        StartCoroutine(ScreenWipe(1, 1, 0));
        Time.timeScale = 1;
    }

    // gameobjects call this to move to a new scene with a fun little animation
    public void DoSceneTransition(string sceneName)
    {
        StartCoroutine(NewSceneTransition(0.5f, sceneName));
        _screenWipe.fillOrigin = 0;
        // Change fillorigin so we move from the other side of the screen when exiting, looks cooler
    }

    // Play screen wipe animation and load scene after delay
    private IEnumerator NewSceneTransition (float transitionLength, string toScene = "MainMenu")
    {
        StartCoroutine(ScreenWipe(transitionLength - 0.1f, 0, 1));
        // We set the screen wipe time to be slightly shorter than the waitforseconds to prevent timing issues
        yield return new WaitForSecondsRealtime(transitionLength);
        LoadScene(toScene);
    }

    private IEnumerator ScreenWipe(float duration, float startPos = 0, float endPos = 1)
    {
        _screenWipe.fillAmount = startPos;

        for (float f = 0; f < 1; f += Time.unscaledDeltaTime / duration)
        {
            var fill = Mathf.Lerp(startPos, endPos, f);
            _screenWipe.fillAmount = fill;
            yield return null;
        }
        _screenWipe.fillAmount = endPos;
    }
    // Does a screen wipe to black
}
