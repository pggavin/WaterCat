using System.Collections;
using UnityEngine;

public class NewLevelOnDefeat : MonoBehaviour
{
    private void OnDisable()
    {
        if (GetComponent<Health>().curHealth <= 0)
            OnDefeated();
        // When enemy is disabled they should be dead
        // but we check for health since the player can also disable them by going to the menu
    }

    private void OnDefeated()
    {
        SoundSystem.Instance.PlaySound("BossDefeated");
        LevelSpawner.Instance.ChangeLevel(1.0f);
    }
}