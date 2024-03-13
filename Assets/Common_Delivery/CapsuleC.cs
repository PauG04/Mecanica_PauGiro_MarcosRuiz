using System;
using UnityEngine.UIElements;

[System.Serializable]
public struct CapsuleC
{
    #region FIELDS
    public Vector3C positionA;
    public Vector3C positionB;
    public float radius;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public PlaneC IsInside(Vector3C point)
    {
        Vector3C v = new Vector3C(positionA, positionB);
        Vector3C u = new Vector3C(positionA, point);

        float dotProduct = Vector3C.Dot(u, v.normalized);

        Vector3C midPoint = positionA + (v.normalized * dotProduct);

        Vector3C heigth = new Vector3C(midPoint, point);

        Vector3C collisionPoint = midPoint + (heigth.normalized * radius);

        return new PlaneC(collisionPoint, heigth.normalized);
    }
    #endregion

    #region FUNCTIONS
    #endregion

}