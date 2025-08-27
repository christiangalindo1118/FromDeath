using UnityEngine;

public class FollowCamera : MonoBehaviour
{
<<<<<<< HEAD
    [Header("Configuración Básica")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.3f; // Tiempo que tarda la cámara en alcanzar al objetivo. Un valor más bajo es más rápido.

    [Header("Opciones de Seguimiento")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    [Header("Límites de Cámara")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero; // Requerido por SmoothDamp

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
        else
        {
            Debug.LogError("¡No se ha asignado un objetivo para la cámara!", this);
        }
=======
    #region Inspector Variables
    [Header("Configuración Básica")]
    [SerializeField] private Transform target;         // Objetivo a seguir (jugador)
    [SerializeField] private float smoothing = 5f;     // Velocidad de suavizado

    [Header("Opciones de Seguimiento")]
    [Tooltip("Habilita el seguimiento horizontal")]
    [SerializeField] private bool followX = true;
    
    [Tooltip("Habilita el seguimiento vertical")]
    [SerializeField] private bool followY = true;

    [Header("Límites de Cámara")]
    [SerializeField] private bool useBounds = false;   // Activar límites de movimiento
    [SerializeField] private Vector2 minPosition;      // Esquina inferior izquierda
    [SerializeField] private Vector2 maxPosition;      // Esquina superior derecha
    #endregion

    private Vector3 initialOffset;   // Offset inicial entre cámara y objetivo
    private Vector3 targetPosition;  // Posición calculada para la cámara

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("¡No se ha asignado un objetivo para la cámara!");
            return;
        }

        // Calcular offset inicial basado en posición relativa
        initialOffset = transform.position - target.position;
>>>>>>> db545ec4c4c600415723bcd605423adf833938d9
    }

    void LateUpdate()
    {
        if (target == null) return;

<<<<<<< HEAD
        Vector3 targetPosition = transform.position;
        if (followX) targetPosition.x = target.position.x + offset.x;
        if (followY) targetPosition.y = target.position.y + offset.y;

        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
        }

        // Usamos SmoothDamp para un seguimiento fluido e independiente del framerate.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
        offset = transform.position - target.position;
    }

=======
        // Calcular nueva posición basada en opciones de seguimiento
        targetPosition = transform.position;

        if (followX)
            targetPosition.x = target.position.x + initialOffset.x;

        if (followY)
            targetPosition.y = target.position.y + initialOffset.y;

        // Aplicar límites si están activados
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                minPosition.x,
                maxPosition.x
            );

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                minPosition.y,
                maxPosition.y
            );
        }

        // Aplicar movimiento suavizado
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothing * Time.deltaTime
        );
    }

    #region Métodos Públicos
    // Método para cambiar objetivos durante el juego
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
        initialOffset = transform.position - target.position;
    }

    // Método para actualizar límites dinámicamente
>>>>>>> db545ec4c4c600415723bcd605423adf833938d9
    public void SetCameraBounds(Vector2 newMin, Vector2 newMax)
    {
        minPosition = newMin;
        maxPosition = newMax;
        useBounds = true;
    }
<<<<<<< HEAD
}
=======
    #endregion
}
>>>>>>> db545ec4c4c600415723bcd605423adf833938d9
