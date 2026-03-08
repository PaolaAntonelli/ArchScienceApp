using UnityEngine;
using Structure;

[System.Serializable]
public class Constraints : IPositionable // Nome classe con la 'S'
{
    public string name;
    private Vector3 position;
    public bool movableQ;
    public ConstraintType type;
    public bool internalQ;
    public bool fixedSizes = false;

    public GameObject inputObject = null; 

    // Implementazione interfaccia IPositionable
    public double Position 
    {
        get 
        {
            if (constraintObject != null)
                return (double)constraintObject.transform.localPosition.z;
            return (double)positionRatio;
        }
    }

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

    // FIX: Il nome del costruttore deve essere identico alla classe (Constraints)
    public Constraints(string name, float positionRatio, ConstraintType type, bool internalQ, bool movableQ, float minRatioPosition = 0.0f, float maxRatioPosition = 1.0f)
    {
        this.name = name;
        this.minRatioPosition = minRatioPosition;
        this.maxRatioPosition = maxRatioPosition;
        this.positionRatio = positionRatio; 
        this.type = type;
        this.internalQ = internalQ;
        this.movableQ = movableQ;
    }

    // Metodi richiesti dagli altri script (XRI e MathematicalSegment)
    public float GetPositionRatio() => positionRatio;
    
    public void UpdateRatioFromWorldPosition(Vector3 worldPosition, Vector3 beamStart, Vector3 beamEnd)
    {
        if (!movableQ) return;
        Vector3 beamVec = beamEnd - beamStart;
        float beamLength = beamVec.magnitude;
        if (beamLength < 0.001f) return;

        Vector3 relativePoint = worldPosition - beamStart;
        float projection = Vector3.Dot(relativePoint, beamVec.normalized);
        positionRatio = projection / beamLength;
    }

    public void SetPositionRatio(float ratioIN) => positionRatio = ratioIN;
    public void SetPosition(Vector3 positionIN) => position = positionIN;
    public Vector3 GetPosition() => position;

    private GameObject constraintObject;
    public void SetObject(GameObject obj) => constraintObject = obj;
    public GameObject GetObject() => constraintObject;

    private GameObject commonJointObject;
    public void SetCommonObject(GameObject obj) => commonJointObject = obj;
    public GameObject GetCommonObject() => commonJointObject;

    public dofFreeQ dofFreeQ
    {
        get
        {
            return type switch
            {
                ConstraintType.Roller => new dofFreeQ(true, false, false),
                ConstraintType.Hinge  => new dofFreeQ(false, false, true),
                ConstraintType.Fixed  => new dofFreeQ(false, false, false),
                _                     => new dofFreeQ(false, false, false),
            };
        }
    }
}

public enum ConstraintType { Roller, Hinge, Fixed, RotationLock }

public struct dofFreeQ
{
    public bool x, y, r;
    public dofFreeQ(bool x, bool y, bool r) { this.x = x; this.y = y; this.r = r; }
}