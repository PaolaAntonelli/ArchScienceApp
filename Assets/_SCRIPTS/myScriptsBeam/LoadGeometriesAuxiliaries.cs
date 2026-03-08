using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; 
using Structure;

public class LoadGeometriesAuxiliaries : MonoBehaviour
{
    private LoadSchemeAutomatic loadingScheme;
    private List<Loads> loads = new List<Loads>();
    
    // Uniformato a List<Constraints>
    private List<Constraints> constraints = new List<Constraints>();
    private BeamPositioning beamPosition; 
    
    [Header("Assets & Materials")]
    public LoadGeometriesAuxiliaries loadGeometries;
    public ConstraintsGeometries constraintsGeometries;
    public Material loadMaterial;

    private float scaleFactor = 0;

    void Start()
    {
        loadingScheme = GetComponent<LoadSchemeAutomatic>();
        if (loadingScheme == null)
        {
            Debug.LogError("[XRI-Setup] LoadSchemeAutomatic non trovato su " + gameObject.name);
            return;
        }

        beamPosition = FindBeamPositioning(transform.parent);
    }

    void Update()
    {
        if (loadingScheme != null)
        {
            loads = loadingScheme.GetLoads();
            constraints = loadingScheme.GetConstraints();
        }
        
        UpdateConstraints();
        UpdateLoads();
    }

    // --- LOGICA DI AGGIORNAMENTO ---

    public void UpdateLoads()
    {
        if (beamPosition == null) return;

        Vector3 basePoint = beamPosition.GetBaseCenter();
        Vector3 beamNormal = beamPosition.GetNormal();
        float beamLength = beamPosition.GetBeamLength();

        foreach (Loads load in loads)
        {
            GameObject pointerObject = load.GetPointerObject();
            if (pointerObject == null) continue;

            Vector3 currentPos = pointerObject.transform.position;
            float newRatio = Vector3.Dot(currentPos - basePoint, beamNormal) / beamLength;
            
            load.SetPositionRatio(Mathf.Clamp(newRatio, load.minRatio, load.maxRatio));
            Vector3 snappedPos = basePoint + load.positionRatio * beamLength * beamNormal;
            
            pointerObject.transform.position = snappedPos;
            if (load.GetMagnitudeObject() != null)
                load.GetMagnitudeObject().transform.position = snappedPos;
        }
    }

    public void UpdateConstraints()
    {
        if (beamPosition == null) return;

        Vector3 basePoint = beamPosition.GetBaseCenter();
        Vector3 beamNormal = beamPosition.GetNormal();
        float beamLength = beamPosition.GetBeamLength();

        foreach (Constraints constraint in constraints)
        {
            GameObject obj = constraint.GetObject();
            if (obj == null) continue;

            float newRatio = Vector3.Dot(obj.transform.position - basePoint, beamNormal) / beamLength;
            constraint.positionRatio = Mathf.Clamp(newRatio, constraint.minRatioPosition, constraint.maxRatioPosition);
            
            Vector3 snappedPos = basePoint + constraint.positionRatio * beamLength * beamNormal;
            obj.transform.position = snappedPos;

            if (constraint.GetCommonObject() != null)
                constraint.GetCommonObject().transform.position = snappedPos;
        }
    }

    public void SetConstraints()
    {
        if (beamPosition == null) return;

        Vector3 basePoint = beamPosition.GetBaseCenter();
        Vector3 beamNormal = beamPosition.GetNormal();
        float beamLength = beamPosition.GetBeamLength();
        Vector3 deflDir = beamPosition.GetDeflectionDirection().normalized;

        foreach (Constraints constraint in constraints)
        {
            if (constraint.GetObject() != null) continue;

            Vector3 pos = basePoint + constraint.positionRatio * beamLength * beamNormal;
            GameObject prefab = GetConstraintPrefab(constraint);
            
            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, pos, CalculateRotation(beamNormal, deflDir));
                
                float size = 5 * beamPosition.GetBeamSectionSize();
                newObj.transform.localScale = Vector3.one * size;
                
                constraint.SetObject(newObj);
                SetupXRMovement(newObj, constraint);
                ShowObject(newObj, true);
            }
        }
    }

    // --- UTILITY ---

    private Quaternion CalculateRotation(Vector3 normal, Vector3 deflection)
    {
        if (normal == Vector3.zero) return Quaternion.identity;
        Vector3 zAxis = Vector3.Cross(normal, deflection).normalized;
        if (zAxis == Vector3.zero) zAxis = Vector3.forward;
        Vector3 yAxis = Vector3.Cross(zAxis, normal).normalized;
        return Quaternion.LookRotation(zAxis, yAxis);
    }

    // Cambiato parametro in 'Constraints c'
    private void SetupXRMovement(GameObject obj, Constraints c)
    {
        var mover = obj.GetComponent<Constraints>(); 
        if (mover != null)
        {
            // Nota: assicurati che questi metodi esistano nello script Constraints.cs
            // Se Constraints.cs non ha questi metodi, dovrai aggiungerli o rimuovere queste righe
            // mover.SetBeamDirection(beamPosition.GetNormal());
            // mover.SetBeamLength(beamPosition.GetBeamLength());
            // mover.SetBasePoint(beamPosition.GetBaseCenter());
        }
    }

    // Cambiato parametro in 'Constraints c'
    private GameObject GetConstraintPrefab(Constraints c)
    {
        if (c.inputObject != null) return c.inputObject;
        return c.type switch {
            ConstraintType.Roller => constraintsGeometries.rollerObject,
            ConstraintType.Hinge => constraintsGeometries.hingeObject,
            ConstraintType.Fixed => constraintsGeometries.fixedObject,
            _ => null
        };
    }

    private void ShowObject(GameObject obj, bool isConstraint)
    {
        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
            if (isConstraint) 
            {
                var mats = obj.GetComponent<ConstraintMaterials>();
                if (mats != null && mats.constraintMaterial != null) {
                    renderer.material = mats.constraintMaterial;
                } else {
                    renderer.material = constraintsGeometries.constraintDefaultMaterial;
                }
            } 
            else 
            {
                renderer.material = loadMaterial;
            }
        }
    }

    BeamPositioning FindBeamPositioning(Transform start)
    {
        if (start == null) return null;
        var comp = start.GetComponentInChildren<BeamPositioning>();
        if (comp != null) return comp;
        return start.parent != null ? FindBeamPositioning(start.parent) : null;
    }
}