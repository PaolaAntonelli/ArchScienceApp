using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Structure;
// Rimosso Oculus.Interaction e altri namespace non necessari

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BeamPositioning))]
public class MeshGenerator : MonoBehaviour
{
    [Header("Mesh Settings")]
    public int extrusionSteps = 6;
    public int beamRatio = 8;
    public SectionType sectionType;
    public int beamSegments = 2; 

    [Header("Materials")]
    public Material undeformedMaterial;
    public Material momentumMaterial;
    public Material momentumFillMaterial;

    [Header("State Control")]
    public bool drawBeam = true;
    public bool drawUndeformedMeshQ = false;
    public bool deformMainBeamQ = false;
    public bool drawMomentumMeshQ = false;

    private Mesh mesh;
    private BeamPositioning beamPositioning;
    private LoadGeometriesAuxiliaries loadGeometriesAuxiliaries;
    private LoadSchemeAutomatic loadingScheme;
    private MathematicalSegment mathematicalSegment;

    // Riferimenti agli oggetti della mesh
    private GameObject undeformedMeshObject;
    private GameObject momentumMeshObject;
    private GameObject momentumFillMeshObject;

    void Awake()
    {
        beamPositioning = GetComponent<BeamPositioning>();
        loadGeometriesAuxiliaries = GetComponent<LoadGeometriesAuxiliaries>();
        loadingScheme = GetComponent<LoadSchemeAutomatic>();
        mathematicalSegment = GetComponent<MathematicalSegment>();
    }

    /// <summary>
    /// Genera la mesh della trave estrudendo la sezione lungo l'asse.
    /// Ottimizzato per l'aggiornamento in tempo reale durante l'interazione XR.
    /// </summary>
    public Mesh GenerateMesh()
    {
        if (beamPositioning == null) return null;

        float length = beamPositioning.GetBeamLength();
        // Calcolo dinamico della dimensione della sezione basato sulla trave XR
        float sectionSize = length / beamRatio;
        beamPositioning.SetBeamSectionSize(sectionSize);

        // Recupera la sezione trasversale (Quadrata o Rettangolare)
        Vector3[] baseSection = BaseSection.GetSection(sectionType, length, beamRatio, beamPositioning);
        
        int numVertices = baseSection.Length * (extrusionSteps + 1);
        Vector3[] vertices = new Vector3[numVertices];
        Vector3 normal = beamPositioning.GetNormal();

        // Generazione dei vertici lungo la trave
        for (int i = 0; i <= extrusionSteps; i++)
        {
            float t = (float)i / extrusionSteps;
            Vector3 offset = normal * (t * length);
            
            for (int j = 0; j < baseSection.Length; j++)
            {
                vertices[i * baseSection.Length + j] = baseSection[j] + offset;
            }
        }

        // Generazione dei triangoli (corpo della trave)
        int[] triangles = GenerateTriangles(baseSection.Length);

        // Triangolazione dei "tappi" (estremità) usando EarClipping
        List<int> baseTriangles = EarClipping.Triangulate(baseSection, normal);
        
        // Tappo finale (invertito per la corretta orientazione delle normali)
        Vector3[] capSection = new Vector3[baseSection.Length];
        for (int i = 0; i < baseSection.Length; i++) 
            capSection[i] = baseSection[i] + (normal * length);
            
        List<int> capTriangles = EarClipping.Triangulate(capSection, -normal);
        // Invertiamo l'ordine di avvolgimento per il tappo finale
        for (int i = 0; i < capTriangles.Count; i += 3)
        {
            int temp = capTriangles[i];
            capTriangles[i] = capTriangles[i + 1];
            capTriangles[i + 1] = temp;
        }

        // Assemblaggio finale della mesh
        Mesh newMesh = new Mesh
        {
            name = "XR_Beam_Mesh",
            vertices = vertices,
            triangles = CombineTriangles(triangles, baseTriangles, capTriangles, extrusionSteps, baseSection.Length)
        };

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = newMesh;

        return newMesh;
    }

    private int[] GenerateTriangles(int verticesPerSection)
    {
        int numTriangles = extrusionSteps * verticesPerSection * 6;
        int[] triangles = new int[numTriangles];
        int t = 0;

        for (int i = 0; i < extrusionSteps; i++)
        {
            for (int j = 0; j < verticesPerSection; j++)
            {
                int nextJ = (j + 1) % verticesPerSection;
                int v1 = i * verticesPerSection + j;
                int v2 = i * verticesPerSection + nextJ;
                int v3 = (i + 1) * verticesPerSection + j;
                int v4 = (i + 1) * verticesPerSection + nextJ;

                triangles[t++] = v1; triangles[t++] = v3; triangles[t++] = v2;
                triangles[t++] = v2; triangles[t++] = v3; triangles[t++] = v4;
            }
        }
        return triangles;
    }

    private int[] CombineTriangles(int[] body, List<int> startCap, List<int> endCap, int steps, int vPerSec)
    {
        int[] all = new int[body.Length + startCap.Count + endCap.Count];
        body.CopyTo(all, 0);
        
        for (int i = 0; i < startCap.Count; i++) 
            all[body.Length + i] = startCap[i];

        int offset = body.Length + startCap.Count;
        int vertexOffset = steps * vPerSec;
        for (int i = 0; i < endCap.Count; i++) 
            all[offset + i] = endCap[i];

        return all;
    }
}