using UnityEngine;

/// <summary>
/// Gestisce le proprietà di scaling per gli elementi visivi della trave in XR.
/// </summary>
public class ScalingProperties : MonoBehaviour
{
    [Header("Settings Reference")]
    public PointerSettings pointerSettings;

    [SerializeField]
    private float _pointerSizeScaled = 0.0f;

    /// <summary>
    /// Restituisce la dimensione scalata per i puntatori.
    /// In XR, questa viene spesso moltiplicata per il fattore di scala globale.
    /// </summary>
    public float GetPointerSizeScaled()
    {
        // Se non impostato manualmente, prova a recuperare il valore dai settings globali
        if (_pointerSizeScaled <= 0 && pointerSettings != null)
        {
            return pointerSettings.scaleFactor;
        }
        return _pointerSizeScaled;
    }

    /// <summary>
    /// Imposta la dimensione scalata. Utile per feedback visivi dinamici (es. ingrandimento al tocco).
    /// </summary>
    public void SetPointerSizeScaled(float size)
    {
        _pointerSizeScaled = size;
    }

    // Metodi Start e Update rimossi perché non necessari (ottimizzazione performance XR)
}