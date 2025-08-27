using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("DASH CONFIG")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;

    private bool canDash = true;

    private WaitForSeconds dashDurationWait;
    private WaitForSeconds dashCooldownWait;

    void Awake()
    {
        dashDurationWait = new WaitForSeconds(dashDuration);
        dashCooldownWait = new WaitForSeconds(dashCooldown);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        float originalGravity = Player.Instance.RB.gravityScale;
        Player.Instance.RB.gravityScale = 0f; 
        
        Vector2 dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        Player.Instance.RB.linearVelocity = dashDirection * dashForce;

        yield return dashDurationWait;

        Player.Instance.RB.gravityScale = originalGravity;
        Player.Instance.RB.linearVelocity = Vector2.zero;

        yield return dashCooldownWait;
        canDash = true;
    }
}