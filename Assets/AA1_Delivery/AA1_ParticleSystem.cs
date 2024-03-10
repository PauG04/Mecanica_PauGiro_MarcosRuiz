using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.ShaderGraph;
using static AA1_ParticleSystem;
using static AA2_Rigidbody;

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

        public float spawnTime;
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
    int poolCascade;
    float time = 0;

    public Particle[] Update(float dt)
    {
        if (time == 0)
        {
            particles = new Particle[settings.pool];
            poolCascade = 0;
        }

        ParticlesLifetime(dt);
        if (settings.spawnsFromCascade)
        {
            SpawnCascade(dt);

        }

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

                CalculateCollision(i);
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
        settingsCascade.spawnTime += (RandomFloatBetweenRange(settingsCascade.maxParticlesPerSecond, settingsCascade.minParticlesPerSecond) * dt);
        if (settingsCascade.spawnTime < 1.0f)
            return;
        settingsCascade.spawnTime -= 1.0f;
        poolCascade++;
        if (poolCascade >= settings.pool)
        {
            poolCascade = 0;
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

        if (!particles[poolCascade].active)
        {
            particles[poolCascade].position = settingsCascade.PointA + (settingsCascade.PointB - settingsCascade.PointA) * RandomFloatBetweenRange(0, 1);
            particles[poolCascade].positionLast = particles[poolCascade].position;              

            particles[poolCascade].force += new Vector3C(
                RandomFloatBetweenRange(settingsCascade.minForce , settingsCascade.maxForce) * direction.x, 
                RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce) * direction.y, 
                RandomFloatBetweenRange(settingsCascade.minForce, settingsCascade.maxForce) * direction.z
                );

            particles[poolCascade].size = 0.03f;
            particles[poolCascade].mass = 1.0f;

            particles[poolCascade].lifeTime = RandomFloatBetweenRange(settingsCascade.minParticleLife, settingsCascade.maxParticleLife);
            particles[poolCascade].active = true;
        }
    }
    
    public float RandomFloatBetweenRange(float min, float max)
    {
        return (float)rnd.NextDouble() * (max - min) + min;
    }

    public void CalculateCollision(int index)
    {
        for (int i = 0; i < settingsCollision.planes.Length; i++)
        {
            float distance;

            float aComponent = settingsCollision.planes[i].ToEquation().A;
            float bComponent = settingsCollision.planes[i].ToEquation().B;
            float cComponent = settingsCollision.planes[i].ToEquation().C;
            float dComponent = settingsCollision.planes[i].ToEquation().D;

            float upValue = (aComponent * particles[index].position.x) +
                (bComponent * particles[index].position.y) +
                (cComponent * particles[index].position.z) +
                dComponent;

            if (upValue < 0.0f)
            {
                upValue *= -1.0f;
            }

            float downValue = (float)Math.Sqrt((aComponent * aComponent) + (bComponent * bComponent) + (cComponent * cComponent));

            distance = upValue / downValue;

            if (distance <= 0.08)
            {
                CollisionReaction(index, i);
            }
        }    
    }

    public void CollisionReaction(int indexParticle, int indexPlane)
    {
        float isolatedDotProduct = (particles[indexParticle].velocity * settingsCollision.planes[indexPlane].normal) / settingsCollision.planes[indexPlane].normal.magnitude;
        Vector3C vector = settingsCollision.planes[indexPlane].normal / settingsCollision.planes[indexPlane].normal.magnitude;

        Vector3C normalVelocity = (vector * isolatedDotProduct);
        Vector3C tangentVelocity = particles[indexParticle].velocity - normalVelocity;
        particles[indexParticle].velocity = -normalVelocity + tangentVelocity;
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
