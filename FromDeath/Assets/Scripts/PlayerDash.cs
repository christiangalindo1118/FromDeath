using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("DASH CONFIG")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private Color debugRayColor = Color.cyan;

    private Rigidbody2D rb;
    private bool canDash = true;

    void Start() => InitializeComponents();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    // ========== INITIALIZATION ==========
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Dash system initialized");
    }

    // ========== DASH MECHANICS ==========
    private IEnumerator PerformDash()
    {
        canDash = false;
        Debug.Log("<color=green>DASH STARTED</color>");
        
        Vector2 originalVelocity = rb.linearVelocity;  // Guardamos la velocidad original
        ApplyDashMovement();
        ToggleCollision(false);

        yield return new WaitForSeconds(dashDuration);
        
        RestoreCollision();
        Debug.Log("<color=red>DASH ENDED</color>");
        
        yield return new WaitForSeconds(dashCooldown);
        ResetDash();
    }

    private void ApplyDashMovement()
    {
        float dashDirection = Mathf.Sign(transform.localScale.x);
        rb.AddForce(new Vector2(dashDirection * dashForce, 0), ForceMode2D.Impulse);
        
        Debug.Log($"Dash direction: {dashDirection}");
        Debug.DrawRay(transform.position, Vector2.right * dashDirection * 2f, debugRayColor, 1f);
    }

    private void ToggleCollision(bool state)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if(playerCollider != null)
        {
            playerCollider.enabled = state;
            Debug.Log($"Collision {(state ? "enabled" : "disabled")}");
        }
    }

    // ========== COLLISION RESTORATION ==========
    private void RestoreCollision()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if(playerCollider != null)
        {
            playerCollider.enabled = true;
            Debug.Log("Collision restored");
        }
    }

    private void ResetDash()
    {
        canDash = true;
        Debug.Log("<color=yellow>DASH READY</color>");
    }

    // ========== EDITOR VISUALIZATION ==========
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        
        Gizmos.color = debugRayColor;
        float direction = transform.localScale.x > 0 ? 1 : -1;
        Gizmos.DrawRay(transform.position, Vector2.right * direction * 2f);
    }
}