using System;
using static AA1_ParticleSystem;

[System.Serializable]
public class AA1_ParticleSystem
{
    [System.Serializable]
    public struct Settings
    {
        public uint pool;
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
        public float lifeTime;
        public float size;
        public float mass;
        public bool active;
        public void AddForce(Vector3C force)
        {
            this.force += force;
        }
    }

    Random rnd = new Random();
    Particle[] particles;
    int pool;
    float time = 0;
    float spawnTime;

    public Particle[] Update(float dt)
    {
        if (time == 0)
        {
            particles = new Particle[settings.pool];
            pool = 0;
            spawnTime = 0.0f;
        }

        ParticlesLifetime(dt);
        if (settings.spawnsFromCascade)
            SpawnCascade(dt);
        else
            SpawnCannon(dt);

        SolverEuler(dt);
        
        time += dt;
        return particles;
    }

    public void ParticlesLifetime(float dt)
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            if (particles[i].active)
            {
                particles[i].lifeTime -= dt;
                if(particles[i].lifeTime <= 0.0f)
                {
                    particles[i].active = false;
                }
            }
        }
    }

    public void SolverEuler(float dt)
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            if (particles[i].active)
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
                particles[i].position = new Vector3C(100.0f, 100.0f, 100.0f);
                particles[i].acceleration = Vector3C.zero;
                particles[i].velocity = Vector3C.zero;
                particles[i].force = Vector3C.zero;
            }
        }
    }

    public void SpawnCascade(float dt)
    {
        spawnTime += (RandomFloatBetweenRange(settingsCannon.maxParticlesPerSecond, settingsCascade.minParticlesPerSecond) * dt);
        if (spawnTime < 1.0f)
            return;
        spawnTime -= 1.0f;
        pool++;
        if (pool >= settings.pool)
        {
            pool = 0;
        }

        Vector3C direction = new Vector3C();

        if (settingsCascade.aleatoryDirection)
        {
            direction.x = RandomFloatBetweenRange(-1, 1);
            direction.y = RandomFloatBetweenRange(-1, 1);
            direction.z = RandomFloatBetweenRange(-1, 1);
        }
        else
        {
            direction.x = settingsCascade.direction.normalized.x;
            direction.y = settingsCascade.direction.normalized.y;
            direction.z = settingsCascade.direction.normalized.z;
        }

        if (!particles[pool].active)
        {
            particles[pool].position = settingsCascade.PointA + (settingsCascade.PointB - settingsCascade.PointA) * RandomFloatBetweenRange(0, 1);
            particles[pool].positionLast = particles[pool].position;              

            particles[pool].force += new Vector3C(
                RandomFloatBetweenRange(settingsCannon.minForce , settingsCascade.maxForce) * direction.x, 
                RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce) * direction.y, 
                RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce) * direction.z
                );

            particles[pool].size = 0.03f;
            particles[pool].mass = 1.0f;

            particles[pool].lifeTime = RandomFloatBetweenRange(settingsCascade.minParticleLife, settingsCascade.maxParticleLife);
            particles[pool].active = true;
        }
    }

    public void SpawnCannon(float dt)
    {
        spawnTime += (RandomFloatBetweenRange(settingsCannon.maxParticlesPerSecond, settingsCannon.minParticlesPerSecond) * dt);
        if (spawnTime < 1.0f)
            return;
        spawnTime -= 1.0f;
        pool++;
        if (pool >= settings.pool)
        {
            pool = 0;
        }

        if (!particles[pool].active)
        {
            particles[pool].position = settingsCannon.Start;
            particles[pool].positionLast = particles[pool].position;

            particles[pool].force += new Vector3C(
                RandomFloatBetweenRange(settingsCannon.minForce, settingsCannon.maxForce) * (float)Math.Sin((double)settingsCannon.angle * (Math.PI / 180.0f)),
                RandomFloatBetweenRange(settingsCannon.minForce, settingsCannon.maxForce),
                RandomFloatBetweenRange(settingsCannon.minForce, settingsCannon.maxForce) * (float)Math.Cos((double)settingsCannon.angle * (Math.PI / 180.0f))
                );

            particles[pool].size = 0.03f;
            particles[pool].mass = 1.0f;

            particles[pool].lifeTime = RandomFloatBetweenRange(settingsCannon.minParticleLife, settingsCannon.maxParticleLife);
            particles[pool].active = true;
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
