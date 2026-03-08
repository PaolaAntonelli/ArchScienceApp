using UnityEngine;
 // Aggiunto per gestire l'interazione XR

public class PointerState : MonoBehaviour
{
    [Header("Visibility Settings")]
    public bool showPointer = true; 
    
    [Header("Target Markers")]
    [Tooltip("Marker del punto iniziale (es. Sfera Start)")]
    public GameObject startMarker; 
    [Tooltip("Marker del punto finale (es. Sfera End)")]
    public GameObject endMarker; 

    [Header("UI Reference")]
    public GameObject startMenu;

    private bool isAllowedToShowPointer = true;

    void Update()
    {
        // Gestione della visibilità basata sullo stato del menu
        if (startMenu != null)
        {
            // Se il menu è attivo, nascondiamo i puntatori per pulizia visiva in VR
            isAllowedToShowPointer = !startMenu.activeSelf;
        }
        
        UpdateMarkersState();
    }

    /// <summary>
    /// Sincronizza lo stato visivo e fisico dei marker.
    /// In XR, disabilitiamo anche l'interazione se il puntatore è nascosto.
    /// </summary>
    private void UpdateMarkersState()
    {
        bool finalState = showPointer && isAllowedToShowPointer;

        if (startMarker != null && endMarker != null)
        {
            // Toggle dei componenti visivi (Renderer)
            ToggleMarkerComponents(startMarker, finalState);
            ToggleMarkerComponents(endMarker, finalState);
        }
    }

    private void ToggleMarkerComponents(GameObject obj, bool state)
    {
        // Disabilita/Abilita la mesh visibile
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = state;

        // Disabilita/Abilita il collider (fondamentale per i raggi XR)
        var collider = obj.GetComponentInChildren<Collider>();
        if (collider != null) collider.enabled = state;

        // Novità XRI: Impedisce all'utente di afferrare l'oggetto se è nascosto
        var interactable = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable>();
        if (interactable != null)
        {
            // Se usiamo XR Interaction Toolkit 3.0+ usiamo l'interazione abilitata/disabilitata
            obj.SetActive(state); 
        }
    }

    // Metodi pubblici per eventi UI o controller
    public void DeactivatePointer() => showPointer = false;
    public void ActivatePointer() => showPointer = true;
}