using System;
using UnityEngine;
using Structure;
public class Equations_IV:MonoBehaviour
{
    public Equations_IV() { }

    /// <summary>
    /// Genera i coefficienti della matrice A e il vettore b per le condizioni al contorno.
    /// Ottimizzato per evitare boxing e allocazioni eccessive in XR.
    /// </summary>
    public (double[] Arow, double brow) Eqn(BC_TYPE eqType, double? z = null, MathematicalSegment.Segment segment = null)
    {
        if (segment == null || !z.HasValue)
            throw new ArgumentException("Parametri segment o z mancanti.");

        // Estrazione valori dal segmento per chiarezza
        double E = segment.E;
        double I = segment.I;
        double A = segment.A;
        double p = segment.p;
        double q = segment.q;

        // Routing delle equazioni basato sul tipo di condizione al contorno
        return eqType switch
        {
            BC_TYPE.v or BC_TYPE.delta_v => VFun(z.Value, E, I, q),
            BC_TYPE.v1 or BC_TYPE.phi or BC_TYPE.delta_phi => V1Fun(z.Value, E, I, q),
            BC_TYPE.M or BC_TYPE.delta_M => M(z.Value, E, I, q),
            BC_TYPE.T or BC_TYPE.delta_T => T(z.Value, E, I, q),
            BC_TYPE.v4 => V4Fun(z.Value, E, I, q),
            BC_TYPE.w or BC_TYPE.delta_w => WFun(z.Value, E, A, p),
            BC_TYPE.N or BC_TYPE.delta_N => NFun(z.Value, E, A, p),
            BC_TYPE.w2 => W2Fun(z.Value, E, A, p),
            _ => throw new ArgumentException($"Tipo BC non supportato: {eqType}")
        };
    }

    // --- Equazioni della linea elastica (Flessione) ---

    public (double[] Arow, double brow) VFun(double z, double E, double I, double q)
    {
        double EI = E * I;
        return (new double[] { (Math.Pow(z, 3) / 6) / EI, (Math.Pow(z, 2) / 2) / EI, z / EI, 1 / EI, 0, 0 }, 
                -1 * (q * Math.Pow(z, 4)) / (24 * EI));
    }

    public (double[] Arow, double brow) V1Fun(double z, double E, double I, double q)
    {
        double EI = E * I;
        return (new double[] { (Math.Pow(z, 2) / 2) / EI, z / EI, 1 / EI, 0, 0, 0 }, 
                -1 * (q * Math.Pow(z, 3)) / (6 * EI));
    }

    public (double[] Arow, double brow) M(double z, double E, double I, double q)
    {
        return (new double[] { -z, -1, 0, 0, 0, 0 }, (q * Math.Pow(z, 2)) / 2);
    }

    public (double[] Arow, double brow) T(double z, double E, double I, double q)
    {
        return (new double[] { -1, 0, 0, 0, 0, 0 }, q * z);
    }

    public (double[] Arow, double brow) V4Fun(double z, double E, double I, double q)
    {
        return (new double[] { 0, 0, 0, 0, 0, 0 }, -q / (E * I));
    }

    // --- Equazioni assiali ---

    public (double[] Arow, double brow) WFun(double z, double E, double A, double p)
    {
        double EA = E * A;
        return (new double[] { 0, 0, 0, 0, -z / EA, -1 / EA }, (p * Math.Pow(z, 2)) / (2 * EA));
    }

    public (double[] Arow, double brow) NFun(double z, double E, double A, double p)
    {
        return (new double[] { 0, 0, 0, 0, -1, 0 }, p * z);
    }

    public (double[] Arow, double brow) W2Fun(double z, double E, double A, double p)
    {
        return (new double[] { 0, 0, 0, 0, 0, 0 }, p / (E * A));
    }

    // --- Metodi di estrazione risultati per la Mesh VR ---

    public float Get_v(float distanceAlongBeam, MathematicalSegment.Segment segment)
    {
        double z = (double)distanceAlongBeam - segment.Start;
        // Calcolo della freccia v(z) usando le costanti di integrazione c1..c4 calcolate
        double v = (1.0 / (segment.E * segment.I)) * (segment.qIIIInt + (segment.c1 * Math.Pow(z, 3) / 6.0) + 
                   (segment.c2 * Math.Pow(z, 2) / 2.0) + (segment.c3 * z) + segment.c4);
        return (float)v;
    }

    public float Get_M_normalized(float distanceAlongBeam, MathematicalSegment.Segment segment)
    {
        double z = (double)distanceAlongBeam - segment.Start;
        // Il momento M(z) non dipende da E e I se espresso tramite c1 e c2 in questa formulazione
        double M = segment.c1 * z + segment.c2; 
        return (float)M;
    }
}