using UnityEngine;

public class FollowCamera : MonoBehaviour
{
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
    }

    void LateUpdate()
    {
        if (target == null) return;

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

    public void SetCameraBounds(Vector2 newMin, Vector2 newMax)
    {
        minPosition = newMin;
        maxPosition = newMax;
        useBounds = true;
    }
}
