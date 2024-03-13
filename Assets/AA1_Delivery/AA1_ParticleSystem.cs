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
        public float particleMass;
        public float particleSize;
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

        public float collisionFactor;
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
                particles[i].positionLast = particles[i].position;

                particles[i].force += settings.gravity;
                particles[i].acceleration = particles[i].force / particles[i].mass;
                particles[i].velocity += particles[i].acceleration * dt;
                particles[i].position += particles[i].velocity * dt;
             
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
        spawnTime += (RandomFloatBetweenRange(settingsCascade.maxParticlesPerSecond, settingsCascade.minParticlesPerSecond) * dt);
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

            particles[pool].size = settings.particleSize;
            particles[pool].mass = settings.particleMass;

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
                RandomFloatBetweenRange(settingsCannon.minForce, settingsCannon.maxForce) * (float)Math.Sin(RandomFloatBetweenRange(-settingsCannon.angle * ((float)Math.PI / 180.0f), settingsCannon.angle * ((float)Math.PI / 180.0f))),
                RandomFloatBetweenRange(settingsCannon.minForce, settingsCannon.maxForce),
                RandomFloatBetweenRange(settingsCannon.minForce, settingsCannon.maxForce) * (float)Math.Sin(RandomFloatBetweenRange(-settingsCannon.angle * ((float)Math.PI / 180.0f), settingsCannon.angle * ((float)Math.PI / 180.0f)))
                );

            particles[pool].size = settings.particleSize;
            particles[pool].mass = settings.particleMass;

            particles[pool].lifeTime = RandomFloatBetweenRange(settingsCannon.minParticleLife, settingsCannon.maxParticleLife);
            particles[pool].active = true;
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
            double distancePlane;

            Vector3C vector = particles[index].position - settingsCollision.planes[i].position;
            distancePlane = Vector3C.Dot(settingsCollision.planes[i].normal, vector);

            if(distancePlane <= 0)
            {
                CollisionReactionPlanes(index, settingsCollision.planes[i]);
            }
        }

        for (int i = 0; i < settingsCollision.spheres.Length; i++)
        {
            double distanceSphere;
            PlaneC plane = settingsCollision.spheres[i].IsInside(particles[index].position);

            Vector3C vector = particles[index].position - plane.position;
            distanceSphere = Vector3C.Dot(plane.normal, vector);

            if (distanceSphere <= 0)
            {
                CollisionReactionPlanes(index, plane);
            }
        }

        for (int i = 0; i < settingsCollision.capsules.Length; i++)
        {
            double distanceCapsule;
            PlaneC plane = settingsCollision.capsules[i].IsInside(particles[index].position);

            Vector3C vector = particles[index].position - plane.position;
            distanceCapsule = Vector3C.Dot(plane.normal, vector);

            if (distanceCapsule <= 0)
            {
                CollisionReactionPlanes(index, plane);
            }
        }
    }

    public void CollisionReactionPlanes(int indexParticle, PlaneC plane)
    {
        LineC line = new LineC(particles[indexParticle].positionLast, particles[indexParticle].position);
        particles[indexParticle].position = plane.IntersectionWithLine(line);

        float isolatedDotProduct = (particles[indexParticle].velocity * plane.normal) / plane.normal.magnitude;

        Vector3C normalVelocity = plane.normal * isolatedDotProduct;
        Vector3C tangentVelocity = particles[indexParticle].velocity - normalVelocity;
        particles[indexParticle].velocity = (-normalVelocity + tangentVelocity) * settings.bounce;
    }

    public void CollisionReactionSphere(int indexParticle, int indexSphere) 
    {
        LineC line = new LineC(particles[indexParticle].positionLast, particles[indexParticle].position);
       // particles[indexParticle].position = settingsCollision.spheres[indexSphere].IntersectionWithLine(line);
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
