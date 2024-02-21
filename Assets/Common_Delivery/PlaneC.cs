using System;
using static UnityEngine.UI.Image;

[System.Serializable]
public struct PlaneC
{
    #region FIELDS
    public Vector3C position;
    public Vector3C normal;
    #endregion

    #region PROPIERTIES
    public static PlaneC right { get { return new PlaneC(new Vector3C(0, 0, 0), new Vector3C(1, 0, 0)); } }
    public static PlaneC up { get { return new PlaneC(new Vector3C(0, 0, 0), new Vector3C(0, 1, 0)); } }
    public static PlaneC forward { get { return new PlaneC(new Vector3C(0, 0, 0), new Vector3C(0, 0, 1)); } }
    #endregion

    #region CONSTRUCTORS
    public PlaneC(Vector3C position, Vector3C normal)
    {
        this.position = position;
        this.normal = normal.normalized;
    }
    public PlaneC(Vector3C pointA, Vector3C pointB, Vector3C pointC)
    {
        Vector3C vectorAB = new Vector3C(pointA, pointB); // Vector from A to B
        Vector3C vectorAC = new Vector3C(pointA, pointC); // Vector from A to C

        this.normal = Vector3C.Cross(vectorAB, vectorAC).normalized;

        this.position = pointA;
    }
    public PlaneC(float A, float B, float C, float D)
    {
        this.position = new Vector3C(-D / A, -D / B, -D / C);
        this.normal = new Vector3C(A, B, C);
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    public (float A, float B, float C, float D) ToEquation()
    {
        return (normal.x, normal.y, normal.z, -position.x * normal.x);
    }

    public Vector3C NearestPoint(Vector3C point)
    {
        Vector3C vector = point - position;
        float dot = Vector3C.Dot(vector, normal);
        Vector3C nearestPoint = position - normal * dot;

        return nearestPoint;
    }
    public Vector3C IntersectionWithLine(LineC line)
    {
        return new Vector3C();
    }
    public override bool Equals(object obj)
    {
        if (obj is PlaneC)
        {
            PlaneC other = (PlaneC)obj;
            return other.normal == this.normal;
        }
        return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}