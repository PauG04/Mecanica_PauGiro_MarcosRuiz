using System;
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

        public Vector3C[] vertexPositions;

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

        public void CalculateVertexPositions()
        {
            vertexPositions[0] = new Vector3C(position.x + size.x / 2, position.y + size.y / 2, position.z + size.z / 2);
            vertexPositions[1] = new Vector3C(position.x - size.x / 2, position.y + size.y / 2, position.z + size.z / 2);
            vertexPositions[2] = new Vector3C(position.x + size.x / 2, position.y + size.y / 2, position.z - size.z / 2);
            vertexPositions[3] = new Vector3C(position.x - size.x / 2, position.y + size.y / 2, position.z - size.z / 2);
            vertexPositions[4] = new Vector3C(position.x + size.x / 2, position.y - size.y / 2, position.z + size.z / 2);
            vertexPositions[5] = new Vector3C(position.x - size.x / 2, position.y - size.y / 2, position.z + size.z / 2);
            vertexPositions[6] = new Vector3C(position.x + size.x / 2, position.y - size.y / 2, position.z - size.z / 2);
            vertexPositions[7] = new Vector3C(position.x - size.x / 2, position.y - size.y / 2, position.z - size.z / 2);

            MatrixC xMatrix = MatrixC.RotateX(euler.x);
            for(int i = 0; i < vertexPositions.Length; i++)
            {
                vertexPositions[i].x = (xMatrix * vertexPositions[i]).x;
            }
        }

        public void CalculateCollision(PlaneC[] planes, float bounce)
        {
            double distancePlane;
            CalculateVertexPositions();

            for (int i = 0; i < planes.Length; i++)
            {
                for (int j = 0; j < vertexPositions.Length; j++)
                {
                    Vector3C vector = vertexPositions[j] - planes[i].position;
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
            Vector3C lastVertexDirection = (lastPosition - position).normalized;
            Vector3C lastVertexPosition = vertex + (lastVertexDirection * Math.Abs(Vector3C.Dot(lastPosition, position)));

            Vector3C newVertexPosition = plane.IntersectionWithLine(new LineC(lastVertexPosition, vertex));
            Vector3C newPosition = position + (lastVertexDirection * Math.Abs(Vector3C.Dot(vertex, newVertexPosition)));
            position = newPosition;

            float isolatedDotProduct = (linearVelocity * plane.normal) / plane.normal.magnitude;

            Vector3C normalVelocity = plane.normal * isolatedDotProduct;
            Vector3C tangentVelocity = linearVelocity - normalVelocity;
            linearVelocity = (-normalVelocity + tangentVelocity) * bounce;
        }

        public void Update(float dt, Vector3C gravity, PlaneC[] planes, float bounce)
        {
            Euler(dt, gravity);
            CalculateCollision(planes, bounce);
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
    }
}
