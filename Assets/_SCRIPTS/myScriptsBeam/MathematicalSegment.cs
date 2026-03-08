using System;
using System.Collections.Generic;
using UnityEngine;
using Structure; 

public class MathematicalSegment : MonoBehaviour
{
    [System.Serializable]
    public class Segment
    {
        // Rimosso da qui perché il singolo segmento non contiene la lista di tutti i segmenti
        [Header("Segment Info")]
        public int Index;            
        public double Start;         
        public double End;           
        public double Length => End - Start;

        [Header("Material & Section")]
        public double E, I, A, p, q;

        [Header("Integration Constants")]
        public double c1, c2, c3, c4, c5, c6;
        public double qIIIInt; 

        [Header("Boundary Conditions")]
        public List<BoundaryCondition> BCs = new List<BoundaryCondition>();

        public Segment(int index, double start, double end)
        {
            this.Index = index;
            this.Start = start;
            this.End = end;
            this.BCs = new List<BoundaryCondition>();
        }
    }

    [Header("Beam Segments Data")]
    public List<Segment> segments = new List<Segment>();

    // --- POSIZIONE CORRETTA: Fuori da Segment, dentro MathematicalSegment ---
    public List<Segment> GetSegments() 
    { 
        return segments; 
    }

    public Segment GetSegmentAt(double z)
    {
        foreach (var s in segments)
        {
            if (z >= s.Start && z <= s.End) return s;
        }
        return null;
    }
}