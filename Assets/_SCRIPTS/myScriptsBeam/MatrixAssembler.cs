using System;
using System.Collections.Generic;
using System.Linq;
using Structure;
using UnityEngine;

public class MatrixAssembler:MonoBehaviour
{
    private readonly Equations_IV eqns = new Equations_IV();

    /// <summary>
    /// Assembla la matrice globale di rigidezza e il vettore dei carichi.
    /// Ottimizzato per il ricalcolo dinamico in ambienti XR.
    /// </summary>
    public MatrixResult AssembleMatrix(List<MathematicalSegment.Segment> segments)
    {
        if (segments == null || segments.Count == 0) return null;

        int noSegments = segments.Count;
        const int noCoeff = 6; // Coefficienti c1-c6 per ogni segmento
        int noUnknowns = noSegments * noCoeff;

        // Calcola il numero totale di condizioni al contorno (BC) per definire le righe della matrice
        int noBcs = segments.Sum(segment => segment.BCs.Count);

        double[,] matrix = new double[noBcs, noUnknowns];
        double[] vector = new double[noBcs];

        int currentRow = 0;

        for (int sIndex = 0; sIndex < segments.Count; sIndex++)
        {
            var segment = segments[sIndex];
            
            foreach (var bc in segment.BCs)
            {
                BC_TYPE bcType = bc.Type;
                double z = (bc.Position == ConstraintPosition.End) ? segment.Length : 0;
                double value = bc.Value;

                // Richiama le equazioni differenziali dalla classe EQ_IV
                var result = eqns.Eqn(bcType, z: z, segment: segment);
                
                if (!bc.InternalQ) 
                { 
                    // Condizioni esterne (vincoli o carichi puntuali agli estremi)
                    for (int i = 0; i < noCoeff; i++)
                    {
                        int colIndex = segment.Index * noCoeff + i;
                        matrix[currentRow, colIndex] = result.Arow[i];
                    }
                    vector[currentRow] = result.brow + value;
                }
                else
                {
                    // Condizioni Interne (Continuità tra segmenti o salti di sollecitazione)
                    // Applica i coefficienti al segmento attuale
                    for (int i = 0; i < noCoeff; i++)
                    {
                        int colIndex = segment.Index * noCoeff + i;
                        matrix[currentRow, colIndex] = result.Arow[i];
                    }
                    
                    double brow_temp = result.brow;

                    // Ricerca ottimizzata del segmento precedente senza LINQ pesante per le performance XR
                    if (sIndex > 0)
                    {
                        var previousSegment = segments[sIndex - 1];
                        double previousPosition = previousSegment.Length;
                        
                        // Calcola l'equazione per la fine del segmento precedente
                        var prevResult = eqns.Eqn(bcType, z: previousPosition, segment: previousSegment);
                        
                        for (int i = 0; i < noCoeff; i++)
                        {
                            int colIndex = (segment.Index - 1) * noCoeff + i;
                            matrix[currentRow, colIndex] = -prevResult.Arow[i]; // Differenza per la continuità
                        }
                        
                        brow_temp -= prevResult.brow;
                        brow_temp += value;
                        vector[currentRow] = brow_temp;
                    }
                }
                currentRow++;
            }
        }

        return new MatrixResult { Matrix = matrix, Vector = vector };
    }
}

public class MatrixResult
{
    public double[,] Matrix { get; set; }
    public double[] Vector { get; set; }
}