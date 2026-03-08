using UnityEngine;
using Structure;

[System.Serializable]
public class Loads : IPositionable
{
    [Header("Dati Identificativi")]
    public string name;       
    public LoadType type; 
    public bool movableQ = true; 
    public float magnitude = 1;   
    public bool scalableQ = true;
    public bool internalQ = false; 

    [Header("Limiti Moltiplicatore Forza")]
    public float minMultiplier = 0.0f;
    public float maxMultiplier = 2f;

    [Header("Visual Settings (Assegna il Prefab qui)")]
    public GameObject loadPrefab; // <--- LA CASELLA CHE CERCAVI

    [Header("Limiti Posizionamento sulla Trave")]
    [SerializeField] private float _positionRatio;
    public float positionRatio
    {
        get => _positionRatio;
        set => _positionRatio = Mathf.Clamp(value, _minRatioPosition, _maxRatioPosition);
    }

    [SerializeField] private float _minRatioPosition = 0.0f;
    public float minRatioPosition
    {
        get => _minRatioPosition;
        set => _minRatioPosition = value;
    }

    [SerializeField] private float _maxRatioPosition = 1.0f;
    public float maxRatioPosition
    {
        get => _maxRatioPosition;
        set => _maxRatioPosition = value;
    }

    // Riferimenti agli oggetti istanziati (nascosti o gestiti dal codice)
    private GameObject pointerObject; 
    private GameObject magnitudeObject; 
    private Vector3 originalPosition;

    // Proprietà di utility per compatibilità con altri script
    public float minRatio => _minRatioPosition;
    public float maxRatio => _maxRatioPosition;

    // Implementazione Interfaccia IPositionable per la Matrice
    public double Position 
    {
        get 
        {
            if (pointerObject != null)
                return (double)pointerObject.transform.localPosition.z;
            return (double)positionRatio; 
        }
    }

    // Costruttore completo
    public Loads(string name, double positionRatio, LoadType type, int magnitude, bool internalQ, float minMultiplier, float maxMultiplier, float minRatioPosition = 0.0f, float maxRatioPosition = 1.0f)
    {
        this.name = name;
        this.minRatioPosition = minRatioPosition;
        this.maxRatioPosition = maxRatioPosition;
        this.positionRatio = (float)positionRatio; 
        this.type = type;
        this.magnitude = magnitude;
        this.internalQ = internalQ;
        this.minMultiplier = minMultiplier;
        this.maxMultiplier = maxMultiplier;
    }

    // --- METODI DI LOGICA ---

    public void UpdateRatioFromXR(Vector3 worldPos, Vector3 beamStart, Vector3 beamEnd)
    {
        if (!movableQ) return;
        Vector3 beamVec = beamEnd - beamStart;
        float len = beamVec.magnitude;
        if (len < 0.001f) return;
        float projection = Vector3.Dot(worldPos - beamStart, beamVec.normalized);
        positionRatio = projection / len;
    }

    public void SetPositionRatio(float ratioIN) => positionRatio = ratioIN;
    public float GetPositionRatio() => positionRatio;

    // Gestione Oggetti Grafici
    public void SetPointerObject(GameObject obj) => pointerObject = obj;
    public GameObject GetPointerObject() => pointerObject;
    public void SetMagnitudeObject(GameObject obj) => magnitudeObject = obj;
    public GameObject GetMagnitudeObject() => magnitudeObject;
}

// Enum per definire la natura del carico
public enum LoadType { Force, Moment, Distributed }