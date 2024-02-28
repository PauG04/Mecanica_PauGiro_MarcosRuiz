using System;
using static AA1_ParticleSystem;

[System.Serializable]
public class AA1_ParticleSystem
{
    [System.Serializable]
    public struct Settings
    {
        public float pool;
        public Vector3C gravity;
        public float bounce;
        public bool spawnsFromCascade;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCascade
    {
        public Vector3C PointA;
        public Vector3C PointB;
        public Vector3C direction;
        public bool aleatoryDirection;
        public float minForce;
        public float maxForce;
        public float minParticlesPerSecond;
        public float maxParticlesPerSecond;
        public float minParticleLife;
        public float maxParticleLife;
    }
    public SettingsCascade settingsCascade;

    [System.Serializable]
    public struct SettingsCannon
    {
        public Vector3C Start;
        public Vector3C Direction;
        public float angle;
        public float minForce;
        public float maxForce;
        public float minParticlesPerSecond;
        public float maxParticlesPerSecond;
        public float minParticleLife;
        public float maxParticleLife;
    }
    public SettingsCannon settingsCannon;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
        public SphereC[] spheres;
        public CapsuleC[] capsules;
    }
    public SettingsCollision settingsCollision;

    public struct Particle
    {
        public Vector3C position;
        public Vector3C positionLast;
        public Vector3C velocity;
        public Vector3C acceleration;
        public Vector3C force;
        public float particleLife;
        public float size;
        public float mass;
        public bool active;
        public bool isInitialized;

        public Particle(bool isActive)
        {
            position = Vector3C.zero;
            positionLast = Vector3C.zero;
            velocity = Vector3C.zero;
            acceleration = Vector3C.zero;
            force = Vector3C.zero;
            particleLife = 10.0f;
            size = 0.03f;
            mass = 1.0f;
            active = isActive;
            isInitialized = true;
        }

        public void AddForce(Vector3C force)
        {
            this.force += force;
        }
    }

    Random rnd = new Random();
    Particle[] particles = new Particle[1000];

    public Particle[] Update(float dt)
    {
        if (settings.spawnsFromCascade)
        {
            InitCascade();
        }

        SolverEuler(dt);
        

        return particles;
    }

    public void SolverEuler(float dt)
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            if(particles[i].active)
            {
                particles[i].force += settings.gravity;
                particles[i].acceleration = particles[i].force / particles[i].mass;
                particles[i].velocity += particles[i].acceleration * dt;
                particles[i].position += particles[i].velocity * dt;

                particles[i].positionLast = particles[i].position;
                particles[i].force = Vector3C.zero;
            }
            else
            {
                particles[i].position = new Vector3C(1000.0f, 1000.0f, 1000.0f);
                particles[i].acceleration = Vector3C.zero;
                particles[i].velocity = Vector3C.zero;
                particles[i].force = Vector3C.zero;
            }
            
        }
    }

    public void InitCascade()
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            if (!particles[i].isInitialized)
            {
                particles[i] = new Particle(true);
                particles[i].position = settingsCascade.PointA + (settingsCascade.PointB - settingsCascade.PointA) * RandomFloatBetweenRange(0, 1);
                particles[i].positionLast = particles[i].position;
                //particles[i].force += new Vector3C(0.0f, RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce), 0.0f);
                particles[i].force += new Vector3C(RandomFloatBetweenRange(settingsCascade.minForce , settingsCascade.maxForce) * RandomFloatBetweenRange(0, 1), RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce) * RandomFloatBetweenRange(0, 1), RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce) * RandomFloatBetweenRange(0, 1));
                particles[i].particleLife = RandomFloatBetweenRange(settingsCascade.minParticleLife, settingsCascade.maxParticleLife);
                particles[i].isInitialized = true;
            }
        }
    }
    
    public float RandomFloatBetweenRange(float min, float max)
    {
        return (float)rnd.NextDouble() * (max - min) + min;
    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
        foreach (var item in settingsCollision.capsules)
        {
            item.Print(Vector3C.green);
        }
        foreach (var item in settingsCollision.spheres)
        {
            item.Print(Vector3C.blue);
        }
    }
}
