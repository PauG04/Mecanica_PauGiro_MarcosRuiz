using UnityEngine;

[System.Serializable]
public class AA2_Rigidbody
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
    }
    public SettingsCollision settingsCollision;

    public struct CubeRigidbody
    {
        public Vector3C position;
        public Vector3C euler;
        public Vector3C size;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
        public float inertialTension;
        public float density;

        public CubeRigidbody(Vector3C _position, Vector3C _size, Vector3C _euler)
        {
            position = _position;
            size = _size;
            euler = _euler;
            linearVelocity = Vector3.zero;
            angularVelocity = Vector3.zero;
            inertialTension = 0;
            density = 0;
        }
    }
    public CubeRigidbody crb = new CubeRigidbody(Vector3C.zero, new(.1f,.1f,.1f), Vector3C.zero);
    public void Update(float dt)
    {

    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
    }
}
