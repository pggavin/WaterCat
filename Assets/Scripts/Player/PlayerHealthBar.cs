using PlayerScripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerHealthBar : MonoBehaviour
{
    private Image _healthBar;
    private Image _slowHealthBar;

    private float _playerHealth = 1.0f;
    // Represents percentage of player health left

    private void Start()
    {
        _healthBar = transform.GetChild(0).GetComponent<Image>();
        _slowHealthBar = GetComponent<Image>();
        
        Watercat.current._healthBar = this;
    }

    // health bar positions are lerped to player health %
    private void Update()
    {
        // Health bar is composed of two images, one which tracks health quickly
        // And another which tracks slower, but is layered under the fast one
        var fastLerp = Mathf.Lerp(_healthBar.fillAmount,     _playerHealth, Time.deltaTime * FAST_LERP_SPEED);
        var slowLerp = Mathf.Lerp(_slowHealthBar.fillAmount, _playerHealth, Time.deltaTime * SLOW_LERP_SPEED);

        _healthBar    .fillAmount = fastLerp;
        _slowHealthBar.fillAmount = slowLerp;
    }

    // Called by player to update the health used for lerps
    internal void UpdateHealth(float healthPercent)
    {
        _playerHealth = healthPercent;
    }

    const float SLOW_LERP_SPEED = 3.0f;
    const float FAST_LERP_SPEED = 9.0f;
}
