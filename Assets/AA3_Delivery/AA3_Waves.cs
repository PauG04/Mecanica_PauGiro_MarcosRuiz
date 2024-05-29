using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class AA3_Waves
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
    }
    public Settings settings;
    [System.Serializable]
    public struct WavesSettings
    {
        public float amplitude;
        public float frequency;
        public float phase;

        public Vector3C direction;
        public float speed;
    }
    public WavesSettings[] wavesSettings;
    public struct Vertex
    {
        public Vector3C originalposition;
        public Vector3C position;
        public Vertex(Vector3C _position)
        {
            this.position = _position;
            this.originalposition = _position;
        }
    }
    public Vertex[] points;

    [System.Serializable]
    public struct BuoySettings
    {
        public float buoyancyCoeeficient;
        public float buoyVelocity;
        public float mass;
        public float waterDensity;
        public float gravity;
    }
    public BuoySettings buoySettings;


    public SphereC buoy;

    private float elapsedTime;

    public AA3_Waves()
    {
        elapsedTime = 0.0f;
    }

    public float GetWaveHeight(float x, float z)
    {
        float Yposition = 0;
        for (int j = 0; j < wavesSettings.Length; j++)
        {
            float k = 2 * (float)Math.PI / wavesSettings[j].frequency;
            Yposition += wavesSettings[j].amplitude
            * (float)Math.Sin(k * (Vector3C.Dot(buoy.position, wavesSettings[j].direction) + elapsedTime * wavesSettings[j].speed)
            + wavesSettings[j].phase);
        }

        return Yposition;
    }

    public void Update(float dt)
    {
        elapsedTime += dt;

        for (int i = 0; i < points.Length; i++)
        {
            points[i].position = points[i].originalposition;

            for (int j = 0; j < wavesSettings.Length; j++)
            {
                float k = 2 * (float)Math.PI / wavesSettings[j].frequency;

                points[i].position.x += points[i].originalposition.x + wavesSettings[j].amplitude * k
                    * (float)Math.Cos(k * (Vector3C.Dot(points[i].originalposition, wavesSettings[j].direction) + elapsedTime * wavesSettings[j].speed)
                    + wavesSettings[j].phase) * wavesSettings[j].direction.x;

                points[i].position.z += points[i].originalposition.z + wavesSettings[j].amplitude * k
                    * (float)Math.Cos(k * (Vector3C.Dot(points[i].originalposition, wavesSettings[j].direction) + elapsedTime * wavesSettings[j].speed)
                    + wavesSettings[j].phase) * wavesSettings[j].direction.z;

                points[i].position.y += wavesSettings[j].amplitude
                    * (float)Math.Sin(k * (Vector3C.Dot(points[i].originalposition, wavesSettings[j].direction) + elapsedTime * wavesSettings[j].speed)
                    + wavesSettings[j].phase);
            }
        }

        float waveHeight = GetWaveHeight(buoy.position.x, buoy.position.z);

        float inmersiveHeight = waveHeight - buoy.position.y - buoy.radius;

        float volume = ((float)Math.PI * (float)Math.Pow(inmersiveHeight, 2) / 3) * (3 * buoy.radius - inmersiveHeight);

        float force = buoySettings.waterDensity * buoySettings.gravity * volume;

        float finalForce = (force - (buoySettings.mass * buoySettings.gravity)) * buoySettings.buoyancyCoeeficient;

        float acceleration = finalForce / buoySettings.mass;

        buoySettings.buoyVelocity += acceleration * dt;

        buoy.position.y += buoySettings.buoyVelocity * dt;
    }

    public void Debug()
    {
        if(points != null)
        foreach (var item in points)
        {
            item.originalposition.Print(0.05f);
            item.position.Print(0.05f);
        }

        buoy.Print(Vector3C.blue);
    }
}
