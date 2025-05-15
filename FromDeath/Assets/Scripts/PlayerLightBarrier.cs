using UnityEngine;

public class PlayerLightBarrier : MonoBehaviour
{
    public static bool HasLightBarrier { get; private set; }

    [SerializeField] private GameObject barrierVFX; // Efecto visual del escudo

    void Start() => DeactivateBarrier();

    public void ActivateBarrier(float duration)
    {
        HasLightBarrier = true;
        barrierVFX.SetActive(true);
        Invoke(nameof(DeactivateBarrier), duration);
    }

    private void DeactivateBarrier()
    {
        HasLightBarrier = false;
        barrierVFX.SetActive(false);
    }
}
