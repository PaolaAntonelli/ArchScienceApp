using UnityEngine;
using Structure; // Questo permette di vedere 'Position' definita in GlobalDefinitions

public class RescalePointer : MonoBehaviour
{
    public PointerSettings pointerSettings; 
    public Position position; // Usa l'enum definito globalmente

    void Start()
    {
        if (pointerSettings == null)
        {
            Debug.LogError($"[RescalePointer] PointerSettings non assegnato su {gameObject.name}");
            return;
        }

        // 1. Applica la scala definita nel profilo XR
        float s = pointerSettings.scaleFactor;
        transform.localScale = new Vector3(s, s, s);

        // 2. Posizionamento iniziale
        // Nota: Assicurati che in PointerSettings.cs i campi si chiamino baseInitialPosition e endInitialPosition
        if (position == Position.Start)
        {
            transform.localPosition = pointerSettings.baseInitialPosition;
        }
        else
        {
            transform.localPosition = pointerSettings.endInitialPosition;
        }
    }
}

// NOTA: L'enum Position è stato rimosso da qui perché ora è in GlobalDefinitions.cs