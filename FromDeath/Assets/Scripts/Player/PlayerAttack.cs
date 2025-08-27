using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private CircleCollider2D attackCollider; 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float hitboxDuration = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;

    private bool canAttack = true;
    private WaitForSeconds hitboxWait;
    private WaitForSeconds cooldownWait;

    private void Awake()
    {
        hitboxWait = new WaitForSeconds(hitboxDuration);
        cooldownWait = new WaitForSeconds(attackCooldown);
        if (attackCollider != null) attackCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;
        // Reproducimos el sonido de ataque
        AudioManager.Instance.PlaySFX(attackSound);
        attackCollider.enabled = true;
        yield return hitboxWait;
        attackCollider.enabled = false;
        yield return cooldownWait;
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
            { 
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }
    // ... (resto del script) ...
}
