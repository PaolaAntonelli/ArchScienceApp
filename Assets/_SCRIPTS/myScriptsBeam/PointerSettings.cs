using UnityEngine;

[CreateAssetMenu(fileName = "PointerSettings", menuName = "StructuralVR/PointerSettings", order = 1)]
public class PointerSettings : ScriptableObject
{
    [Header("Scaling & Visuals")]
    [Tooltip("Fattore di scala per i puntatori e gli indicatori visivi nel visore.")]
    public float scaleFactor = 0.0005f; 

    [Header("Default Beam Positioning")]
    [Tooltip("Posizione iniziale del punto di origine (Base) della trave in coordinate mondo.")]
    public Vector3 baseInitialPosition = new Vector3(-1.2f, 0.0f, 2f); 

    [Tooltip("Posizione iniziale del punto finale (End) della trave in coordinate mondo.")]
    public Vector3 endInitialPosition = new Vector3(1.2f, 0.0f, 2f); 

    /// <summary>
    /// Configura le posizioni iniziali dei marker della trave.
    /// Utile per resettare la scena VR o posizionare la trave davanti all'utente all'avvio.
    /// </summary>
    public void SetPointerInitialLocations(Vector3 basePosition, Vector3 endPosition)
    {
        baseInitialPosition = basePosition;
        endInitialPosition = endPosition;
        
        // In ambiente di sviluppo, segna l'oggetto come "sporco" per salvare le modifiche
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}