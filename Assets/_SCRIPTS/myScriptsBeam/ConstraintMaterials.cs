using UnityEngine;

public class ConstraintMaterials : MonoBehaviour
{
    [Header("Visual Settings")]
    public Material constraintMaterial;      // Materiale standard
    public Material highlightedMaterial;     // Materiale quando viene toccato/selezionato
    public Color constraintColor = Color.white;
}