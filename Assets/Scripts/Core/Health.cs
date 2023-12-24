using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    internal int curHealth;
    // cur health is set to max health every time this object is enabled.
    private IOnDeath onDeath;
    private IOnDamaged onDamage;
    // there should be scripts on the gameobject that implement these
    [SerializeField]
    private bool isEnemy = true;

    private void Awake()
    {
        onDeath = GetComponent<IOnDeath>();
        onDamage = GetComponent<IOnDamaged>();
    }

    private void OnEnable()
    {
        curHealth = maxHealth;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    // Proactive fix for any bugs that can be caused from enemy
    // being disabled with an active coroutine

    public void NegateHealth(int damage)
    {
        curHealth -= damage;
        if (curHealth > 0)
            onDamage.Damaged(curHealth);
        else
        {
            onDeath.Died();
            VisualEffects.Instance.DoParticleExplosion(transform.position);
            gameObject.SetActive(false);
            // cool particle fx on death
        }
    }
    // Just negates health

    public bool GetIsEnemy() => isEnemy;
}