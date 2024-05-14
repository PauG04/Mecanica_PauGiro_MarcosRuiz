using System;
using System.ComponentModel;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

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

    [System.Serializable]
    public struct CubeRigidbody
    {
        public Vector3C position;
        public Vector3C lastPosition;
        public Vector3C euler;
        public Vector3C force;
        public Vector3C size;
        public Vector3C linearVelocity;
        public Vector3C angularVelocity;
        public Vector3C acceleration;
        public float inertialTension;
        public float density;
        public float mass;

        private Vector3C[] vertexPositions;

        public CubeRigidbody(Vector3C _position, Vector3C _size, Vector3C _euler)
        {
            position = _position;
            lastPosition = Vector3C.zero;
            size = _size;
            euler = _euler;
            force = Vector3C.zero;
            linearVelocity = Vector3C.zero;
            angularVelocity = Vector3C.zero;
            acceleration = Vector3C.zero;
            inertialTension = 0;
            density = 1f;
            mass = .1f;
            vertexPositions = new Vector3C[8];

            vertexPositions[0] = new Vector3C(-size.x / 2, size.y / 2, size.z / 2);
            vertexPositions[1] = new Vector3C(-size.x / 2, -size.y / 2, size.z / 2);
            vertexPositions[2] = new Vector3C(-size.x / 2, size.y / 2, -size.z / 2);
            vertexPositions[3] = new Vector3C(-size.x / 2, -size.y / 2, -size.z / 2);
            vertexPositions[4] = new Vector3C(size.x / 2, size.y / 2, size.z / 2);
            vertexPositions[5] = new Vector3C(size.x / 2, -size.y / 2, size.z / 2);
            vertexPositions[6] = new Vector3C(size.x / 2, size.y / 2, -size.z / 2);
            vertexPositions[7] = new Vector3C(size.x / 2, -size.y / 2, -size.z / 2);
        }
        public void Euler(float dt, Vector3C gravity)
        {
            lastPosition = position;
            acceleration = (force / mass) + gravity;
            linearVelocity += acceleration * dt;
            euler += angularVelocity * dt;
            position += linearVelocity * dt;

            force = Vector3C.zero;
        }

        public void CalculateCollision(PlaneC[] planes, float bounce)
        {
            double distancePlane;

            for (int i = 0; i < planes.Length; i++)
            {
                for (int j = 0; j < vertexPositions.Length; j++)
                {
                    Vector3C vector = (vertexPositions[j] + position) - planes[i].position;
                    distancePlane = Vector3C.Dot(planes[i].normal, vector);

                    if (distancePlane <= 0)
                    {
                        CollisionReaction(planes[i], bounce, vertexPositions[j]);
                    }
                }
            }
        }

        public void CollisionReaction(PlaneC plane, float bounce, Vector3C vertex)
        {
            float dot = Vector3C.Dot(plane.normal, lastPosition - plane.position);
            position = plane.IntersectionWithLine(new LineC(lastPosition, position)) + plane.normal * dot;

            Vector3C normalVelocity = plane.normal * Vector3C.Dot(linearVelocity, plane.normal);
            linearVelocity = ((linearVelocity - normalVelocity) - normalVelocity) * bounce;
        }

        public void Update(float dt, Vector3C gravity, PlaneC[] planes, float bounce)
        {
            Euler(dt, gravity);
            CalculateCollision(planes, bounce);
        }

        public Vector3C[] GetVertex()
        {
            return vertexPositions;
        }
    }
    public CubeRigidbody crb = new CubeRigidbody(Vector3C.zero, new(.1f,.1f,.1f), Vector3C.zero);
    public void Update(float dt)
    {
        crb.Update(dt, settings.gravity, settingsCollision.planes, settings.bounce);
    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }

        foreach (var vertex in crb.GetVertex())
        {
            SphereC sphere = new SphereC(vertex + crb.position, 0.01f);
            sphere.Print(Vector3C.green);
        }
    }
}
