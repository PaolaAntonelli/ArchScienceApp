using UnityEngine;

public class UpdateShaderProperties : MonoBehaviour
{
    public Material targetMaterial;
    private LoadSchemeAutomatic loadingScheme;
    private MathematicalSegment mathematicalSegment;
    private BeamPositioning beamPositioning;

    [SerializeField] private Color[] mycolors = { 
        new Color32(255, 255, 255, 255),
        new Color32(255, 255, 255, 255),
        new Color32(255, 255, 255, 255),
        new Color32(136, 118, 89, 255),
        new Color32(109, 76, 61, 255),
        new Color32(220, 201, 182, 255),
        new Color32(114, 125, 113, 255) 
    };

    void Start()
    {
        loadingScheme = GetComponent<LoadSchemeAutomatic>();
        mathematicalSegment = GetComponent<MathematicalSegment>();
        beamPositioning = GetComponent<BeamPositioning>();
    }

    void Update()
    {
        if (targetMaterial == null || loadingScheme == null) return;

        // Recuperiamo i dati calcolati per aggiornare lo shader dei diagrammi
        var segments = mathematicalSegment.segments;
        if (segments == null || segments.Count == 0) return;

        int numThresholds = segments.Count + 1;
        float[] shaderThresholds = new float[10];
        Vector4[] shaderColors = new Vector4[10];

        for (int i = 0; i < numThresholds && i < 10; i++)
        {
            if (i < segments.Count)
                shaderThresholds[i] = (float)segments[i].Start;
            else
                shaderThresholds[i] = (float)segments[i-1].End;

            shaderColors[i] = mycolors[i % mycolors.Length];
        }

        targetMaterial.SetFloatArray("_Thresholds", shaderThresholds);
        targetMaterial.SetVectorArray("_Colors", shaderColors);
        targetMaterial.SetInt("_NumThresholds", numThresholds);
    }
}