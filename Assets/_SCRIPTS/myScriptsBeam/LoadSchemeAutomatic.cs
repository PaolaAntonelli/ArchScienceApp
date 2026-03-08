using System.Collections.Generic;
using UnityEngine;
using System;

// Rimosse le dipendenze Oculus.Interaction e Unity.Loading non necessarie
public class LoadSchemeAutomatic : MonoBehaviour
{
    [Header("Material Properties")]
    public float E = 1; // Modulo di elasticità
    public float A = 1; // Area della sezione
    public float I = 1; // Momento d'inerzia

    [Header("Structural Elements")]
    // Utilizziamo 'Constraint' (singolare) come definito in Constraints.cs
    public List<Constraints> constraints = new List<Constraints>(); 

    // Utilizziamo 'Loads' (plurale) per coerenza con il file Loads.cs aggiornato
    public List<Loads> loads = new List<Loads>(); 
    //aggiunta a posteriori
    [Header("Internal State")]
    private BeamPositioning beamPosition; 
    private float scaleFactor = 0;
    private float loadOriginalZLength = 1f;

    // --- Metodi di Accesso (Getters) ---

    public List<Loads> GetLoads()
    {
        return loads;
    }

    public List<Constraints> GetConstraints()
    {
        return constraints;
    }

    // --- Metodi di Configurazione (Setters) ---

    public void SetLoads(List<Loads> loadIN)
    {
        loads = loadIN;
    }

    public void SetConstraints(List<Constraints> constraintsIN)
    {
        constraints = constraintsIN;
    }

    /// <summary>
    /// Metodo di utilità per inizializzare il riferimento alla trave in ambiente XR.
    /// </summary>
    private void Awake()
    {
        // Cerchiamo il componente BeamPositioning nella gerarchia dell'oggetto XR
        beamPosition = GetComponentInParent<BeamPositioning>();
        
        if (beamPosition == null)
        {
            beamPosition = FindFirstObjectByType<BeamPositioning>();
        }
    }
}