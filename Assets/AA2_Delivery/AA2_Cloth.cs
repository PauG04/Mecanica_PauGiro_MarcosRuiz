using UnityEngine;

[System.Serializable]
public class AA2_Cloth
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        [Min(1)]
        public float width;
        [Min(1)]
        public float height;
        [Min(2)]
        public int xPartSize;
        [Min(2)]
        public int yPartSize;
    }
    public Settings settings;
    [System.Serializable]
    public struct ClothSettings
    {
        [Header("Structural Spring")]
        public float structuralElasticCoef;
        public float structuralDamptCoef;
        public float structuralSpringL;

        [Header("Shear Spring")]
        public float shearElasticCoef;
        public float shearDamptCoef;
        public float shearSpringL;

        [Header("Bending Spring")]
        public float bendingElasticCoef;
        public float bendingDamptCoef;
        public float bendingSpringL;
    }
    public ClothSettings clothSettings;

    [System.Serializable]
    public struct SettingsCollision
    {
        public SphereC sphere;
    }
    public SettingsCollision settingsCollision;
    public struct Vertex
    {
        public Vector3C lastPosition;
        public Vector3C actualPosition;
        public Vector3C velocity;
        public Vertex(Vector3C _position)
        {
            this.actualPosition = _position;
            this.lastPosition = _position;
            this.velocity = new Vector3C(0, 0, 0);
        }
        public void Euler(Vector3C force, float dt)
        {
            lastPosition = actualPosition;
            velocity += force * dt;
            actualPosition += velocity * dt;
        }
    }
    public Vertex[] points;
    public void Update(float dt)
    {
        int xVertices = settings.xPartSize + 1;
        //int yVertices = settings.yPartSize + 1;

        Vector3C[] forces = new Vector3C[points.Length];

        ApplyForces(xVertices, forces);

        for (int i = 0; i < points.Length; i++)
        {
            if (i != 0 && i != xVertices - 1)
                points[i].Euler(settings.gravity + forces[i], dt);
        }

    }

    private void ApplyForces(int xVertices, Vector3C[] forces)
    {
        for (int i = 0; i < points.Length; i++)
        {
            StructuralForce(i, xVertices, forces);
            ShearForce(i, xVertices, forces);
            BendingForce(i, xVertices, forces);
        }
    }

    private void StructuralForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        HorizontalStructuralForce(currentParticle, xVertices, forces);
        VerticalStructuralForce(currentParticle, xVertices, forces);
    }

    private void ShearForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        if (currentParticle > xVertices - 1 && currentParticle % xVertices - 1 != 0)
        {
            float shearMagnitude = (points[currentParticle - xVertices + 1].actualPosition - points[currentParticle].actualPosition).magnitude
                                             - clothSettings.shearSpringL;
            Vector3C shearForceVector = (points[currentParticle - xVertices + 1].actualPosition
                                - points[currentParticle].actualPosition).normalized * shearMagnitude * clothSettings.shearElasticCoef;


            Vector3C shearDampingForce = (points[currentParticle - xVertices + 1].actualPosition - points[currentParticle].actualPosition) * clothSettings.shearDamptCoef;
            Vector3C shearSpringForce = shearForceVector * clothSettings.shearElasticCoef - shearDampingForce;

            forces[currentParticle] += shearSpringForce;
            forces[currentParticle - xVertices + 1] += -shearSpringForce;
        }
    }

    private void BendingForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        HorizontalBendingForce(currentParticle, xVertices, forces);
        VerticalBendingForce(currentParticle, xVertices, forces);
    }

    private void VerticalStructuralForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        if (currentParticle > xVertices - 1)
        {
            float structMagnitudeY = (points[currentParticle - xVertices].actualPosition - points[currentParticle].actualPosition).magnitude
                                             - clothSettings.structuralSpringL;
            Vector3C structForceVector = (points[currentParticle - xVertices].actualPosition
                                - points[currentParticle].actualPosition).normalized * structMagnitudeY * clothSettings.structuralElasticCoef;

            Vector3C structDampingForce = (points[currentParticle - xVertices].actualPosition - points[currentParticle].actualPosition) * clothSettings.structuralDamptCoef;
            Vector3C structSpringForce = structForceVector * clothSettings.structuralElasticCoef - structDampingForce;


            forces[currentParticle] += structSpringForce;
            forces[currentParticle - xVertices] += -structSpringForce;
        }
    }

    private void HorizontalStructuralForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        if (currentParticle % xVertices != 0)
        {
            float structMagnitudeX = (points[currentParticle - 1].actualPosition - points[currentParticle].actualPosition).magnitude
                                             - clothSettings.structuralSpringL;
            Vector3C structForceVector = (points[currentParticle - 1].actualPosition
                                - points[currentParticle].actualPosition).normalized * structMagnitudeX * clothSettings.structuralElasticCoef;

            Vector3C structDampingForce = (points[currentParticle - 1].actualPosition - points[currentParticle].actualPosition) * clothSettings.structuralDamptCoef;
            Vector3C structSpringForce = structForceVector * clothSettings.structuralElasticCoef - structDampingForce;

            forces[currentParticle] += structSpringForce;
            forces[currentParticle - 1] += -structSpringForce;
        }
    }

    private void VerticalBendingForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        if (currentParticle > xVertices * 2 - 1)
        {
            float bendMagnitudeY = (points[currentParticle - xVertices * 2].actualPosition - points[currentParticle].actualPosition).magnitude
                                             - clothSettings.bendingSpringL;
            Vector3C bendForceVector = (points[currentParticle - xVertices * 2].actualPosition
                                - points[currentParticle].actualPosition).normalized * bendMagnitudeY * clothSettings.bendingElasticCoef;

            Vector3C bendDampingForce = (points[currentParticle - xVertices * 2].actualPosition - points[currentParticle].actualPosition) * clothSettings.bendingDamptCoef;
            Vector3C bendSpringForce = bendForceVector * clothSettings.bendingElasticCoef - bendDampingForce;


            forces[currentParticle] += bendSpringForce;
            forces[currentParticle - xVertices * 2] += -bendSpringForce;
        }
    }

    private void HorizontalBendingForce(int currentParticle, int xVertices, Vector3C[] forces)
    {
        if (currentParticle % xVertices != 0 && currentParticle % xVertices != 1)
        {
            float bendMagnitudeX = (points[currentParticle - 2].actualPosition - points[currentParticle].actualPosition).magnitude
                                             - clothSettings.bendingSpringL;
            Vector3C bendForceVector = (points[currentParticle - 2].actualPosition
                                - points[currentParticle].actualPosition).normalized * bendMagnitudeX * clothSettings.bendingElasticCoef;

            Vector3C bendDampingForce = (points[currentParticle - 2].actualPosition - points[currentParticle].actualPosition) * clothSettings.bendingDamptCoef;
            Vector3C bendSpringForce = bendForceVector * clothSettings.bendingElasticCoef - bendDampingForce;


            forces[currentParticle] += bendSpringForce;
            forces[currentParticle - 2] += -bendSpringForce;
        }
    }

    public void Debug()
    {
        settingsCollision.sphere.Print(Vector3C.blue);

        if (points != null)
            foreach (var item in points)
            {
                item.lastPosition.Print(0.05f);
                item.actualPosition.Print(0.05f);
            }
    }
}
