using UnityEngine;
using Structure;

public class BaseSection
{
    public static Vector3[] GetSection(SectionType sectionType, float extrusionLength, int beamRatio, BeamPositioning beamPositioning)
    {
        // Calcolo delle dimensioni basato sui parametri della trave
        float width = extrusionLength / beamRatio;
        float height = extrusionLength / beamRatio;

        // Generazione dei vertici "flat" (2D)
        Vector3[] vertices = (sectionType == SectionType.Squared) 
            ? GetSquareSection(width) 
            : GetRectangleSection(width, height);

        // Trasformazione nello spazio 3D basata sui dati di posizionamento XRI
        return TransformSection(vertices, beamPositioning.GetBaseCenter(), beamPositioning.GetNormal());
    }

    private static Vector3[] GetSquareSection(float size)
    {
        float half = size / 2f;
        return new Vector3[]
        {
            new Vector3(-half, -half, 0),
            new Vector3(half, -half, 0),
            new Vector3(half, half, 0),
            new Vector3(-half, half, 0)
        };
    }

    private static Vector3[] GetRectangleSection(float width, float height)
    {
        float halfW = width / 2f;
        float halfH = height / 2f;
        return new Vector3[]
        {
            new Vector3(-halfW, -halfH, 0),
            new Vector3(halfW, -halfH, 0),
            new Vector3(halfW, halfH, 0),
            new Vector3(-halfW, halfH, 0)
        };
    }

    /// <summary>
    /// Trasforma la sezione piana nella posizione e orientamento corretti.
    /// Ottimizzato per seguire la normale calcolata dagli interactor XR.
    /// </summary>
    private static Vector3[] TransformSection(Vector3[] section, Vector3 center, Vector3 normal)
    {
        // LookRotation è più affidabile in VR per orientare oggetti lungo una traiettoria
        Quaternion rotation = Quaternion.LookRotation(normal);

        for (int i = 0; i < section.Length; i++)
        {
            section[i] = (rotation * section[i]) + center;
        }

        return section;
    }
}