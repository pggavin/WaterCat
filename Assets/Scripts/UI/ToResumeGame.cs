using UnityEngine;
using UnityEngine.EventSystems;

public class ToResumeGame : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PauseControl.Instance.UnpauseGame();
    }
}
