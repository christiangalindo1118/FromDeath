using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private CircleCollider2D attackCollider; // Arrastra el CircleCollider2D aquí
    [SerializeField] private LayerMask enemyLayer;

    private bool canAttack = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        // Activa el hitbox temporalmente
        attackCollider.enabled = true;
        yield return new WaitForSeconds(0.1f); // Duración del hitbox
        attackCollider.enabled = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Detecta enemigos al entrar en el trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
            else
            {
                Debug.LogWarning("Objeto en layer 'Enemy' sin componente Enemy: " + other.name);
            }
        }
    }

    // Dibuja el Gizmo del hitbox
    void OnDrawGizmos()
    {
        if (attackCollider != null && attackCollider.enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                attackCollider.transform.position + (Vector3)attackCollider.offset,
                attackCollider.radius
            );
        }
    }
}