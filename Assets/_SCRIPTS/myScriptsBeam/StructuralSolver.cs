using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnityEngine; // Aggiunto per i log di Unity

/// <summary>
/// Risolutore statico per sistemi lineari complessi.
/// Utilizza Math.NET Numerics per garantire precisione e velocità nel calcolo strutturale XR.
/// </summary>
public static class MatrixSolver
{
    public static double[] Solve(double[,] matrix, double[] vector)
    {
        try
        {
            // Converte la matrice e il vettore di input nei tipi Math.NET
            var A = DenseMatrix.OfArray(matrix);
            var b = DenseVector.OfArray(vector);

            // Risolve il sistema lineare Ax = b
            // Math.NET sceglie automaticamente l'algoritmo migliore (es. LU o QR)
            var x = A.Solve(b);

            // Ritorna i coefficienti come array di double
            return x.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[StructuralSolver] Errore durante la risoluzione della matrice: {ex.Message}");
            return null;
        }
    }
}