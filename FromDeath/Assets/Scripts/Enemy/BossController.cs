using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyHealth))]
public class BossController : MonoBehaviour
{
    public enum BossState { Disappear, MeleeAttack, RangedAttack, SpecialAbility }

    [Header("Main Configuration")]
    [SerializeField] private float disappearTime = 3f;
    [SerializeField] private float vulnerableTime = 2f;

    [Header("Abilities")]
    [SerializeField] private GameObject[] abilityPrefabs;
    [SerializeField] private Transform[] attackPoints;
    
    [Header("Animations")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string[] attackTriggers;

    private EnemyHealth enemyHealth;
    private BossState currentState;
    private bool isVulnerable = true;
    private Collider2D bossCollider;
    private DamageArea damageArea; // Restaurado

    private WaitForSeconds disappearWait;
    private WaitForSeconds vulnerableWait;
    private WaitForSeconds attackWait;
    private WaitForSeconds meleeWait1;
    private WaitForSeconds meleeWait2;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        bossCollider = GetComponent<Collider2D>();
        damageArea = GetComponentInChildren<DamageArea>(); // Restaurado
        if (bossAnimator == null) bossAnimator = GetComponent<Animator>();

        disappearWait = new WaitForSeconds(disappearTime);
        vulnerableWait = new WaitForSeconds(vulnerableTime);
        attackWait = new WaitForSeconds(1f);
        meleeWait1 = new WaitForSeconds(0.3f);
        meleeWait2 = new WaitForSeconds(0.2f);

        enemyHealth.OnDeath.AddListener(StartDefeatSequence);
    }

    private void Start()
    {
        StartCoroutine(BehaviorLoop());
    }

    private IEnumerator BehaviorLoop()
    {
        while (true)
        {
            int attackPattern = Random.Range(0, 4);
            switch (attackPattern)
            {
                case 0: yield return StartCoroutine(DisappearAndAttack(BossState.MeleeAttack)); break;
                case 1: yield return StartCoroutine(DisappearAndAttack(BossState.RangedAttack)); break;
                case 2: yield return StartCoroutine(DisappearAndAttack(BossState.SpecialAbility)); break;
                case 3: yield return StartCoroutine(MegaComboAttack()); break;
            }
        }
    }

    private IEnumerator DisappearAndAttack(BossState nextState)
    {
        SetVulnerable(false);
        bossAnimator.SetTrigger("Vanish");
        yield return disappearWait;
        
        // Lógica de reposicionamiento aquí...

        bossAnimator.SetTrigger("Appear");
        yield return attackWait;
        
        ExecuteAttack(nextState);
        yield return vulnerableWait;
        SetVulnerable(true);
    }

    #region Attack System (Restaurado)
    private IEnumerator MegaComboAttack()
    {
        for (int i = 0; i < 4; i++)
        {
            ExecuteAttack((BossState)Random.Range(1, 4));
            yield return attackWait;
        }
    }

    private void ExecuteAttack(BossState attackType)
    {
        currentState = attackType;
        
        switch (attackType)
        {
            case BossState.MeleeAttack: StartCoroutine(MeleeCombo()); break;
            case BossState.RangedAttack: ShootProjectiles(); break;
            case BossState.SpecialAbility: ActivateSpecialAbility(); break;
        }
        
        if (attackTriggers.Length > (int)attackType - 1 && attackTriggers[(int)attackType - 1] != null)
            bossAnimator.SetTrigger(attackTriggers[(int)attackType - 1]);
    }

    private void ShootProjectiles()
    {
        if(abilityPrefabs.Length < 1 || attackPoints.Length < 1) return;

        foreach(Transform point in attackPoints)
        {
            Instantiate(abilityPrefabs[0], point.position, point.rotation);
        }
    }

    private void ActivateSpecialAbility()
    {
        if (abilityPrefabs.Length < 2 || abilityPrefabs[1] == null) return;
        Instantiate(abilityPrefabs[1], transform.position, Quaternion.identity);
    }

    private IEnumerator MeleeCombo()
    {
        for (int i = 0; i < 3; i++)
        {
            if (damageArea != null) damageArea.EnableDamage(true);
            yield return meleeWait1;
            if (damageArea != null) damageArea.EnableDamage(false);
            yield return meleeWait2;
        }
    }
    #endregion

    private void SetVulnerable(bool state)
    {
        isVulnerable = state;
        if (bossCollider != null) bossCollider.enabled = state;
    }

    private void StartDefeatSequence()
    {
        StopAllCoroutines();
        StartCoroutine(DefeatSequence());
    }

    private IEnumerator DefeatSequence()
    {
        if (bossAnimator != null) bossAnimator.SetTrigger("Defeat");
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
