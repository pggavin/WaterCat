using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ToNewScene : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private string _newScene = "MainMenu";

    private bool _pressed = false;

    // This method is called when the user presses the button
    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the button has already been pressed, and if so, return
        if (_pressed)
            return;

        _pressed = true;
        SoundSystem.Instance.PlaySound("StartUp");
        StartCoroutine(DoTransitionAfterDelay());
    }

    // Coroutine to handle the transition to the new scene after a specified delay
    private IEnumerator DoTransitionAfterDelay()
    {
        yield return new WaitForSecondsRealtime(TIME_BEFORE_TRANSITION);
        SceneTransitions.Instance.DoSceneTransition(_newScene);
    }

    const float TIME_BEFORE_TRANSITION = 2.75f;
}