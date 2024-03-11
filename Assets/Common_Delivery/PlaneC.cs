using System;
using System.Runtime.InteropServices.WindowsRuntime;
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
        this.normal = normal;
    }
    public PlaneC(Vector3C pointA, Vector3C pointB, Vector3C pointC)
    {
        Vector3C vectorAB = new Vector3C(pointA, pointB); // Vector from A to B
        Vector3C vectorAC = new Vector3C(pointA, pointC); // Vector from A to C

        this.normal = Vector3C.Cross(vectorAB, vectorAC);

        this.position = pointA;
    }
    public PlaneC(Vector3C n, float D)
    {
        float x, y, z;
        x = -D / -n.x;
        y = -D / -n.y;
        z = -D / -n.z;

        this.position = new Vector3C(x, y, z);
        this.normal = n;
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
        //return (normal.x, normal.y, normal.z, -position.x * normal.x);
        return (normal.x, normal.y, normal.z, -Vector3C.Dot(normal, position));
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
        float distance = Vector3C.Dot(normal, position - line.origin) / Vector3C.Dot(normal, line.direction);
        return line.origin + line.direction * distance;
    }
    public float DistanceToPoint(Vector3C point)
    {
        float planeEquation = ToEquation().A * point.x + ToEquation().B * point.y + ToEquation().C * point.z + ToEquation().D;

        if (normal.magnitude == 0.0f)
            return 0.0f;

        return Math.Abs(planeEquation) / normal.magnitude;
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