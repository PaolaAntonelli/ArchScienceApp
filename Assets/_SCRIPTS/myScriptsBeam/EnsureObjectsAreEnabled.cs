using System.Collections.Generic;
using UnityEngine;

public class EnsureObjectsAreEnabled : MonoBehaviour
{
    [Header("XR Settings")]
    [Tooltip("Lista di oggetti da forzare all'attivazione (es. XR Origin, Interactors, UI)")]
    [SerializeField] public List<GameObject> targetObjects;

    [Tooltip("Se vero, l'attivazione avviene con un leggero ritardo per permettere l'inizializzazione del Rig XR")]
    public bool useDelay = false;

    void Start()
    {
        if (useDelay)
        {
            Invoke(nameof(EnableAll), 0.1f);
        }
        else
        {
            EnableAll();
        }
    }

    public void EnableAll()
    {
        if (targetObjects == null || targetObjects.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] Nessun oggetto assegnato in targetObjects.");
            return;
        }

        foreach (var targetObject in targetObjects)
        {
            if (targetObject != null)
            {
                // Attiva l'oggetto se è disattivato
                if (!targetObject.activeSelf)
                {
                    targetObject.SetActive(true);
                    Debug.Log($"[XRI-Setup] {targetObject.name} è stato attivato correttamente.");
                }
            }
            else
            {
                Debug.LogWarning("[XRI-Setup] Trovato un elemento nullo nella lista targetObjects.");
            }
        }
    }
}