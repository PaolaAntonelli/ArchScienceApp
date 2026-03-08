using UnityEngine;
// Rimosso System.Numerics per evitare conflitti con i Vector3 di Unity

public class BeamPositioning : MonoBehaviour
{
    [Header("Start and end markers")]
    public GameObject start;    // Punto iniziale (es. Controller sinistro o punto fisso)
    public GameObject end;      // Punto finale (es. Controller destro o interactor)
    public GameObject reference; // Punto di riferimento per l'orientamento

    [Header("Beam Properties")]
    private Vector3 referencePoint;
    private Vector3 beamDirection;
    private float beamLength;
    private Vector3 beamVector;
    private Vector3 basePoint;
    private Vector3 endPoint;
    private float beamSectionSize;

    private Vector3 deflectionVector;
    private Transform xrCameraTransform;

    void Start()
    {
        // In XRI, Camera.main punta solitamente alla Main Camera dentro l'XR Origin
        if (Camera.main != null)
        {
            xrCameraTransform = Camera.main.transform;
        }

        UpdateReferencePoint();
        UpdateBeamProperties();
    }

    public void UpdateReferencePoint()
    {
        if (reference != null)
            referencePoint = reference.transform.position;
    }

    public Vector3 GetDeflectionDirection() => deflectionVector;

    public void ComputeDeflectionDirection()
    {
        // Calcola la direzione di "flessione" (puntamento verso il basso relativo alla trave)
        Vector3 globalDown = Vector3.down;

        // Proietta il vettore down sul piano perpendicolare alla direzione della trave
        Vector3 bottomVector = Vector3.ProjectOnPlane(globalDown, beamDirection).normalized;
        
        // Se il vettore risultante guarda verso l'alto (caso limite), lo invertiamo
        if (Vector3.Dot(bottomVector, Vector3.up) > 0)
        {
            bottomVector = -bottomVector;
        }
        
        deflectionVector = bottomVector;
    }

    public void UpdateBeamProperties()
    {
        if (start == null || end == null) return;

        basePoint = start.transform.position;
        endPoint = end.transform.position;

        // Nota: avevi forzato basePoint[1] = 0 (Y = 0). 
        // Se vuoi che la trave sia libera nello spazio VR, commenta le righe sotto.
        // basePoint.y = 0; 
        // endPoint.y = 0;

        UpdateReferencePoint();

        // Logica di orientamento triangolo ABC (Base, Fine, Riferimento)
        Vector3 AB = endPoint - basePoint;
        Vector3 AC = referencePoint - basePoint;

        // Prodotto vettoriale per trovare la normale del piano della trave
        Vector3 normal = Vector3.Cross(AB, AC);
        
        // Orientamento rispetto alla camera XR
        Vector3 camForward = xrCameraTransform != null ? xrCameraTransform.forward : Vector3.forward;
        float orientation = Vector3.Dot(normal.normalized, camForward);
        
        // Se l'orientamento è invertito rispetto all'utente, scambiamo i punti
        if (orientation < 0)
        {
            Vector3 temp = basePoint;
            basePoint = endPoint;
            endPoint = temp;
        }

        beamVector = endPoint - basePoint;
        beamDirection = beamVector.normalized;
        beamLength = beamVector.magnitude;

        ComputeDeflectionDirection();
    }

    // --- Getters e Setters ---
    public float GetBeamSectionSize() => beamSectionSize;
    public void SetBeamSectionSize(float size) => beamSectionSize = size;
    public Vector3 GetBaseCenter() => basePoint;
    public Vector3 GetFinalPoint() => endPoint;
    public Vector3 GetNormal() => beamDirection; // La direzione della trave è la normale per la sezione
    public float GetBeamLength() => beamLength;
    public Vector3 GetBeamVector() => beamVector;
}