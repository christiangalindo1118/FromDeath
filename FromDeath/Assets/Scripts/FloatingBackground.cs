using UnityEngine;

public class FloatingBackground : MonoBehaviour
{
    [Header("MOVIMIENTO VERTICAL")]
    [Tooltip("Velocidad de oscilación del fondo")]
    [SerializeField] private float moveSpeed = 1f;
    
    [Tooltip("Altura máxima del movimiento")]
    [SerializeField][Range(0.1f, 5f)] private float amplitude = 1f;
    
    [Tooltip("Invertir dirección del movimiento")]
    [SerializeField] private bool inverseDirection = false;

    [Header("CONFIGURACIÓN INICIAL")]
    [Tooltip("Offset aleatorio inicial para variación entre objetos")]
    [SerializeField] private bool randomOffset = true;

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = randomOffset ? Random.Range(0f, 2f * Mathf.PI) : 0f;
    }

    void Update()
    {
        float oscillation = Mathf.Sin((Time.time + timeOffset) * moveSpeed) * amplitude;
        float newY = inverseDirection ? startPosition.y - oscillation : startPosition.y + oscillation;
        
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}