using UnityEngine;

namespace Structure 
{
    public interface IPositionable 
    {
        double Position { get; }
        float GetPositionRatio();
    }

    public enum SectionType { Squared, H }
    public enum Position { Start, End }
    public enum ConstraintPosition { Start, End, Internal }

    public enum BC_TYPE 
    { 
        Hinge, Roller, Fixed, CommonJoint,
        v, delta_v, v1, phi, delta_phi, M, delta_M, T, delta_T, v4,
        w, delta_w, N, delta_N, w2 
    }

    [System.Serializable]
    public class BoundaryCondition // <--- Questo risolve gli errori in MatrixAssembler
    {
        public BC_TYPE Type;
        public ConstraintPosition Position;
        public double Value;
        public bool InternalQ;
    }
}