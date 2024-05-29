using System;

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

        public void Update(ref Vertex[] points, float elapsedTime)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].position = points[i].originalposition;

                float k = 2 * (float)Math.PI / frequency;

                points[i].position.x += points[i].originalposition.x + amplitude * k
                    * (float)Math.Cos(k * (Vector3C.Dot(points[i].originalposition, direction) + elapsedTime * speed)
                    + phase) * direction.x;

                points[i].position.z += points[i].originalposition.z + amplitude * k
                    * (float)Math.Cos(k * (Vector3C.Dot(points[i].originalposition, direction) + elapsedTime * speed)
                    + phase) * direction.z;

                points[i].position.y += amplitude
                    * (float)Math.Sin(k * (Vector3C.Dot(points[i].originalposition, direction) + elapsedTime * speed)
                    + phase);

            }
        }
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

        public void Update(ref SphereC buoy, float dt, WavesSettings[] wavesSettings, float elapsedTime)
        {
            Euler(ref buoy, dt, wavesSettings, elapsedTime);
        }

        public float GetWaveHeight(WavesSettings[] wavesSettings, SphereC buoy, float elapsedTime)
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

        public void Euler(ref SphereC buoy, float dt, WavesSettings[] wavesSettings, float elapsedTime)
        {
            float force = waterDensity * gravity * GetVolume(buoy, wavesSettings, elapsedTime);

            float finalForce = (force - (mass * gravity)) * buoyancyCoeeficient;

            float acceleration = finalForce / mass;

            buoyVelocity += acceleration * dt;

            buoy.position.y += buoyVelocity * dt;
        }

        public float GetVolume(SphereC buoy, WavesSettings[] wavesSettings, float elapsedTime)
        {
            float waveHeight = GetWaveHeight(wavesSettings, buoy, elapsedTime);

            float inmersiveHeight = waveHeight - buoy.position.y - buoy.radius;

            return ((float)Math.PI * (float)Math.Pow(inmersiveHeight, 2) / 3) * (3 * buoy.radius - inmersiveHeight);
        }
    }
    public BuoySettings buoySettings;


    public SphereC buoy;

    private float elapsedTime;

    public AA3_Waves()
    {
        elapsedTime = 0.0f;
    }

    public void Update(float dt)
    {
        elapsedTime += dt;
        for (int i = 0; i < wavesSettings.Length; i++)
        {
            wavesSettings[i].Update(ref points, elapsedTime);
        }
        buoySettings.Update(ref buoy, dt, wavesSettings, elapsedTime);
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
