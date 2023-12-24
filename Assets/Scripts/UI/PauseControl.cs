using UnityEngine;
using static UnityEngine.Time;

public class PauseControl : MonoSingleton<PauseControl>
{
    private bool _gamePaused = false;
    private GameObject _toEnable;

    // Get a reference to the pause menu to be enabled when we pause
    protected override void Initialize()
    {
        _toEnable = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_gamePaused && timeScale == 1)
        {
            PauseGame();
        }
    }

    // Pause the game by setting the time scale to 0 while activating the pause menu
    private void PauseGame()
    {
        timeScale = 0;
        _gamePaused = true;
        SoundSystem.Instance.PauseAudio();
        _toEnable.SetActive(true);
    }

    // Unpause the game by setting the time scale to 1 while deactivating the pause menu
    public void UnpauseGame()
    {
        timeScale = 1;
        _gamePaused = !true;
        SoundSystem.Instance.UnpauseAudio();
        _toEnable.SetActive(false);
    }
}
