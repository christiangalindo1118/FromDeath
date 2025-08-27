using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossController : MonoBehaviour
{
    #region Enums
    public enum BossState { Disappear, MeleeAttack, RangedAttack, SpecialAbility }
    #endregion

    #region Inspector Variables
    [Header("Main Configuration")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float disappearTime = 3f;
    [SerializeField] private float vulnerableTime = 2f;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    
    [Header("Abilities")]
    [SerializeField] private GameObject[] abilityPrefabs;
    [SerializeField] private Transform[] attackPoints;
    
    [Header("Animations")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string[] attackTriggers;

    [Header("Movement Boundaries")]
    [SerializeField] private float minX = -50f;  // Límite izquierdo
    [SerializeField] private float maxX = -35f;  // Límite derecho
    [SerializeField] private float minY = -10f;  // Límite inferior
    [SerializeField] private float maxY = 0f;    // Límite superior
    #endregion

    #region Private Variables
    private float currentHealth;
    private BossState currentState;
    private bool isVulnerable = true;
    private int attackPattern = 0;
    private DamageArea damageArea;
    private Collider2D bossCollider;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        InitializeBoss();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        damageArea = GetComponentInChildren<DamageArea>();
        bossCollider = GetComponent<Collider2D>();
        
        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();
    }

    private void InitializeBoss()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        StartCoroutine(BehaviorLoop());
    }
    #endregion

    #region Behavior Coroutines
    private IEnumerator BehaviorLoop()
    {
        while (currentHealth > 0)
        {
           switch (attackPattern % 4)
            {
                case 0:
                    yield return StartCoroutine(DisappearAndAttack(BossState.MeleeAttack));
                    break;
                case 1:
                    yield return StartCoroutine(DisappearAndAttack(BossState.RangedAttack));
                    break;
                case 2:
                    yield return StartCoroutine(DisappearAndAttack(BossState.SpecialAbility));
                    break;
                case 3:
                    yield return StartCoroutine(MegaComboAttack());
                    break;
            }
            
            attackPattern++;
        }
        yield return StartCoroutine(DefeatSequence());
    }

    private IEnumerator DisappearAndAttack(BossState nextState)
    {
        SetVulnerable(false);
        bossAnimator.SetTrigger("Vanish");
        yield return new WaitForSeconds(disappearTime);
        
        //transform.position = GetRandomPosition();
        bossAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(1f);
        
        ExecuteAttack(nextState);
        yield return new WaitForSeconds(vulnerableTime);
    }

    private void ClampPosition()
    {
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX); // Aplica límites X
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY); // Aplica límites Y
        transform.position = clampedPos;
    }
    #endregion

    #region Attack System
    private IEnumerator MegaComboAttack()
    {
        for (int i = 0; i < 4; i++)
        {
            ExecuteAttack((BossState)Random.Range(1, 4));
            yield return new WaitForSeconds(1f);
        }
    }

    private void ExecuteAttack(BossState attackType)
    {
        currentState = attackType;
        
        switch (attackType)
        {
            case BossState.MeleeAttack:
                StartCoroutine(MeleeCombo());
                break;
            case BossState.RangedAttack:
                ShootProjectiles();
                break;
            case BossState.SpecialAbility:
                ActivateSpecialAbility();
                break;
        }
        
        if (attackTriggers.Length > (int)attackType - 1)
            bossAnimator.SetTrigger(attackTriggers[(int)attackType - 1]);
    }

    private void ShootProjectiles()
    {
        if(abilityPrefabs.Length < 1 || attackPoints.Length < 1)
        {
            Debug.LogError("Faltan referencias de ataque a distancia!", gameObject);
            return;
        }

        foreach(Transform point in attackPoints)
        {
            GameObject projectile = Instantiate(
                abilityPrefabs[0], 
                point.position, 
                point.rotation
            );

            // Opcional: Añadir efectos
            //if(bossAnimator != null)
                //bossAnimator.SetTrigger("Shoot");
        }
}

    private void ActivateSpecialAbility()
    {
        if (abilityPrefabs.Length < 2 || abilityPrefabs[1] == null)
        {
            Debug.LogError("Prefab de habilidad especial no asignado", this);
            return;
        }

        Instantiate(abilityPrefabs[1], transform.position, Quaternion.identity);
    
        // Opcional: Añadir efectos adicionales
        if (bossAnimator != null)
            bossAnimator.SetTrigger("SpecialEffect");
    }

    private IEnumerator MeleeCombo()
    {
        for (int i = 0; i < 3; i++)
        {
            if (damageArea != null)
                damageArea.EnableDamage(true);
            
            yield return new WaitForSeconds(0.3f);
            
            if (damageArea != null)
                damageArea.EnableDamage(false);
            
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region Combat System
    public void TakeDamage(float damage)
    {
        if (!isVulnerable || currentHealth <= 0) return;
        
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHealthUI();
        
        if (bossAnimator != null)
            bossAnimator.SetTrigger("Hit");
        
        if (currentHealth <= 0)
            StopAllCoroutines();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth / maxHealth;
    }
    #endregion

    #region Utility Methods
    //private Vector3 GetRandomPosition()
    //{
     //   return new Vector3(
     
     //       Random.Range(-8f, 8f),
       //     Random.Range(-4f, 4f),
       //     0
        //);
    //}

    private void SetVulnerable(bool state)
    {
        isVulnerable = state;
        if (bossCollider != null)
            bossCollider.enabled = state;
    }
    #endregion

    #region Animation Events
    public void EnableAttackCollider(int enable)
    {
        if (damageArea != null)
            damageArea.EnableDamage(enable == 1);
    }
    #endregion

    #region Defeat Sequence
    private IEnumerator DefeatSequence()
    {
        if (bossAnimator != null)
            bossAnimator.SetTrigger("Defeat");
        
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
        
        // Add additional defeat logic here
    }
    #endregion
}

