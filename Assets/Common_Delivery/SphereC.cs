using System;

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
    public bool IsInside()
    {
        return true;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}