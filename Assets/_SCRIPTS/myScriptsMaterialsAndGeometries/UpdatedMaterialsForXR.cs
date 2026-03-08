using UnityEngine;

public class UpdatedMaterialsForXR : MonoBehaviour
{
    public Material restMaterial;
    public Material activeMaterial;
    private Renderer objRenderer;

    void Awake() => objRenderer = GetComponent<Renderer>();

    // Chiamato dall'evento "Hover Entered" del componente XR Grab Interactable
    public void SetHoverActive()
    {
        if (activeMaterial != null) objRenderer.material = activeMaterial;
    }

    // Chiamato dall'evento "Hover Exited"
    public void SetRest()
    {
        if (restMaterial != null) objRenderer.material = restMaterial;
    }
}