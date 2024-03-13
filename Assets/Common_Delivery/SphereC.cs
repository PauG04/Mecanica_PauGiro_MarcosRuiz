using System;
using System.Drawing;

[System.Serializable]
public struct SphereC
{
    #region FIELDS
    public Vector3C position;
    public float radius;
    #endregion

    #region PROPIERTIES

    #endregion

    #region CONSTRUCTORS
    public SphereC(Vector3C position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public PlaneC IsInside(Vector3C point)
    {
        Vector3C planeNormal = new Vector3C(position, point);
        return new PlaneC(planeNormal.normalized * radius + position, planeNormal.normalized);
    }
    #endregion

    #region FUNCTIONS
    #endregion

}