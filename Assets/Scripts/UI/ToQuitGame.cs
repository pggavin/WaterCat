using UnityEngine;
using UnityEngine.EventSystems;

public class ToQuitGame : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Application.Quit();
    }
}