using System;

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
    public PlaneC(Vector3C n, float D)
    {
        this.position = new Vector3C();
        this.normal = new Vector3C();
    }
    #endregion

    #region OPERATORS
    #endregion

    #region METHODS
    #endregion

    #region FUNCTIONS
    #endregion

}