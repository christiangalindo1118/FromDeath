using UnityEngine;

public class PlayerLightBarrier : MonoBehaviour
{
    [SerializeField] private GameObject barrierVFX; // Efecto visual del escudo

    // Propiedad pública para que otros scripts (como DarkZoneController) puedan saber el estado.
    public bool IsBarrierActive { get; private set; }

    void Start()
    {
        // Nos aseguramos de que la barrera empiece desactivada.
        DeactivateBarrier();
    }

    public void ActivateBarrier(float duration)
    {
        if (barrierVFX == null)
        {
            Debug.LogWarning("No se ha asignado el VFX de la barrera en el Inspector.", this);
            return;
        }

        IsBarrierActive = true;
        barrierVFX.SetActive(true);
        
        // Cancelamos cualquier invocación anterior para evitar bugs.
        CancelInvoke(nameof(DeactivateBarrier)); 
        // Usamos Invoke para desactivarla después de la duración. Es simple y efectivo.
        Invoke(nameof(DeactivateBarrier), duration);
    }

    private void DeactivateBarrier()
    {
        IsBarrierActive = false;
        if (barrierVFX != null)
        {
            barrierVFX.SetActive(false);
        }
    }
}