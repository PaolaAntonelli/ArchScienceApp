using System.Collections.Generic;
using UnityEngine;

public static class EarClipping
{
    /// <summary>
    /// Triangola un poligono basandosi sui vertici e la normale.
    /// Ottimizzato per la generazione procedurale in XR.
    /// </summary>
    public static List<int> Triangulate(Vector3[] vertices, Vector3 normal)
    {
        List<int> triangles = new List<int>();
        List<int> indices = new List<int>();

        for (int i = 0; i < vertices.Length; i++)
        {
            indices.Add(i);
        }

        // L'algoritmo riduce il poligono rimuovendo "orecchie" (triangoli convessi)
        while (indices.Count > 2)
        {
            bool earFound = false;

            for (int i = 0; i < indices.Count; i++)
            {
                int prevIndex = indices[(i - 1 + indices.Count) % indices.Count];
                int currentIndex = indices[i];
                int nextIndex = indices[(i + 1) % indices.Count];

                Vector3 prev = vertices[prevIndex];
                Vector3 current = vertices[currentIndex];
                Vector3 next = vertices[nextIndex];

                if (IsEar(prev, current, next, vertices, indices, normal))
                {
                    // In XRI/Unity l'ordine dei triangoli è fondamentale per il culling
                    triangles.Add(prevIndex);
                    triangles.Add(currentIndex);
                    triangles.Add(nextIndex);

                    indices.RemoveAt(i);
                    earFound = true;
                    break;
                }
            }

            if (!earFound)
            {
                // In VR, questo può accadere se i controller creano forme auto-intersecanti
                Debug.LogError("[EarClipping] Impossibile completare la triangolazione. Poligono degenerato o complesso.");
                break;
            }
        }

        return triangles;
    }

    private static bool IsEar(Vector3 prev, Vector3 current, Vector3 next, Vector3[] vertices, List<int> indices, Vector3 normal)
    {
        // 1. Il triangolo deve essere convesso rispetto alla normale della faccia
        if (!IsConvex(prev, current, next, normal))
        {
            return false;
        }

        // 2. Nessun altro vertice del poligono deve trovarsi all'interno del triangolo
        for (int i = 0; i < indices.Count; i++)
        {
            int index = indices[i];
            Vector3 point = vertices[index];

            if (point == prev || point == current || point == next) continue;

            if (IsPointInTriangle(point, prev, current, next))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsConvex(Vector3 a, Vector3 b, Vector3 c, Vector3 normal)
    {
        // Calcola la normale del triangolo corrente
        Vector3 triangleNormal = Vector3.Cross(b - a, c - a);
        // Se il prodotto scalare è positivo, il triangolo segue l'orientamento corretto
        // Usiamo un piccolo epsilon (0.00001f) per la stabilità in VR
        return Vector3.Dot(triangleNormal, normal) > 0.00001f;
    }

    private static bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        // Coordinate baricentriche per determinare se il punto P è nel triangolo ABC
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float denom = (dot00 * dot11 - dot01 * dot01);
        if (Mathf.Abs(denom) < 0.000001f) return false; // Triangolo degenere

        float invDenom = 1 / denom;
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }
}