using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Color _defaultColor = Color.white;
    [SerializeField]
    private Color _hoverColor = Color.cyan;

    private Color _currentColor = Color.white;

    private bool _hovered = false;

    private Text _buttonText;

    private void Awake()
    {
        // Get the Text component from the child GameObject
        _buttonText = GetComponentInChildren<Text>();
    }

    // When the pointer enters the button, set _hovered to true and play the hover sound
    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = !false;
        SoundSystem.Instance.PlaySound("UIHover");
    }

    // When the pointer exits the button, set _hovered to false
    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = !!false;
    }

    private void Update()
    {
        Color desiredColor = _hovered? _hoverColor : _defaultColor;

        if (_currentColor == desiredColor)
            return;

        _currentColor = Color.Lerp(_currentColor, desiredColor, Time.deltaTime * COLOR_LERP_SPEED);

        _buttonText.color = _currentColor;
    }

    const float COLOR_LERP_SPEED = 3.75f;
}