using UnityEngine;

public class DamageArea : MonoBehaviour
{
    #region Inspector Variables
    [Header("CONFIGURACIÓN")]
    [Tooltip("Capa del jugador para detección")]
    [SerializeField] private LayerMask playerLayer;
    
    [Header("DEBUG")]
    [Tooltip("Muestra información de debug en consola")]
    [SerializeField] private bool debugMode = false;
    #endregion

    #region Propiedades
    public bool PlayerInRange { get; private set; }
    #endregion

    #region Componentes
    private Collider2D attackCollider;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HandleTriggerExit(other);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (attackCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, attackCollider.bounds.size);
        }
    }
    #endif
    #endregion

    #region Lógica Principal
    public void EnableDamage(bool state)
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = state;
            if (debugMode) Debug.Log($"Daño {(state ? "activado" : "desactivado")}");
        }
    }
    #endregion

    #region Métodos Privados
    private void InitializeComponents()
    {
        attackCollider = GetComponent<Collider2D>();
        if (attackCollider == null)
        {
            Debug.LogError("No se encontró Collider2D en DamageArea", this);
            return;
        }
        
        attackCollider.isTrigger = true;
        EnableDamage(false);
    }

    private void HandleTriggerEnter(Collider2D other)
    {
        if (playerLayer.Contains(other.gameObject.layer))
        {
            PlayerInRange = true;
            if (debugMode) Debug.Log("Jugador en rango de ataque");
        }
    }

    private void HandleTriggerExit(Collider2D other)
    {
        if (playerLayer.Contains(other.gameObject.layer))
        {
            PlayerInRange = false;
            if (debugMode) Debug.Log("Jugador salió del rango");
        }
    }
    #endregion
}

// Extensión para manejo de capas
public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}