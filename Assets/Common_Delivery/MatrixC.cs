using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

[System.Serializable]
public struct MatrixC
{
    #region FIELDS
    public float[,] data;
    public int size;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS+
    public MatrixC(float[,] data)
    {
        this.data = data;
        size = (int)Math.Sqrt(data.Length);
    }
    #endregion

    #region OPERATORS
    public static Vector3C operator *(MatrixC m, Vector3C a)
    {
        return new Vector3C(m.data[0, 0] * a.x + m.data[0, 1] * a.y + m.data[0, 2] * a.z,
            m.data[1, 0] * a.x + m.data[1, 1] * a.y + m.data[1, 2] * a.z,
            m.data[2, 0] * a.x + m.data[2, 1] * a.y + m.data[2, 2] * a.z);
    }

    public static MatrixC operator *(MatrixC m1, MatrixC m2)
    {
        float[,] results = new float[m1.size, m1.size];

        for (int i = 0; i < m1.size; i++)
        {
            for (int j = 0; j < m2.size; j++)
            {
                for (int k = 0; k < m1.size; k++)
                {
                    results[i, j] += m1.data[i, k] * m2.data[k, j];
                }
            }
        }

        return new MatrixC(results);
    }
    #endregion

    #region METHODS

    public float Determintate3x3(MatrixC m)
    {
        if (m.size != 3)
        {
            return 0;
        }
        float determinate =
            ((m.data[0, 0] * m.data[1, 1] * m.data[2, 2]) + (m.data[0, 1] * m.data[1, 2] * m.data[2, 0]) + (m.data[0, 2] * m.data[1, 0] * m.data[2, 1])) -
            ((m.data[0, 2] * m.data[1, 1] * m.data[2, 0]) + (m.data[0, 0] * m.data[1, 2] * m.data[2, 1]) + (m.data[0, 1] * m.data[1, 0] * m.data[2, 2]));


        return determinate;
    }

    public MatrixC Transposed(MatrixC m)
    {
        MatrixC transposed = new MatrixC();

        for (int i = 0; i < m.size; i++)
        {
            for (int j = 0; j < m.size; j++)
            {
                transposed.data[j, i] = m.data[i, j];
            }
        }

        return transposed;
    }

    #endregion

    #region FUNCTIONS
    public MatrixC Inverse(MatrixC m)
    {
        MatrixC inverse = new MatrixC();

        if (Determintate3x3(m) == 0)
        {
            return new MatrixC();
        }

        float invDet = 1.0f / Determintate3x3(m);

        inverse.data[0, 0] = (m.data[1, 1] * m.data[2, 2] - m.data[1, 2] * m.data[2, 1]) * invDet;
        inverse.data[0, 1] = (m.data[0, 2] * m.data[2, 1] - m.data[0, 1] * m.data[2, 2]) * invDet * -1;
        inverse.data[0, 2] = (m.data[0, 1] * m.data[1, 2] - m.data[0, 2] * m.data[1, 1]) * invDet;
        inverse.data[1, 0] = (m.data[1, 2] * m.data[2, 0] - m.data[1, 0] * m.data[2, 2]) * invDet * -1;
        inverse.data[1, 1] = (m.data[0, 0] * m.data[2, 2] - m.data[0, 2] * m.data[2, 0]) * invDet;
        inverse.data[1, 2] = (m.data[0, 2] * m.data[1, 0] - m.data[0, 0] * m.data[1, 2]) * invDet * -1;
        inverse.data[2, 0] = (m.data[1, 0] * m.data[2, 1] - m.data[1, 1] * m.data[2, 0]) * invDet;
        inverse.data[2, 1] = (m.data[0, 1] * m.data[2, 0] - m.data[0, 0] * m.data[2, 1]) * invDet * -1;
        inverse.data[2, 2] = (m.data[0, 0] * m.data[1, 1] - m.data[0, 1] * m.data[1, 0]) * invDet;

        return Transposed(inverse);
    }

    public static MatrixC Rotate(Vector3C rotation)
    {
        rotation.x *= (float)Math.PI / 180.0f;
        rotation.y *= (float)Math.PI / 180.0f;
        rotation.z *= (float)Math.PI / 180.0f;

        MatrixC matrixRotation = new MatrixC(new float[,]
        {
           {    (float)Math.Cos(rotation.y)*(float)Math.Cos(rotation.z),
                (float)Math.Sin(rotation.x)*(float)Math.Sin(rotation.y)*(float)Math.Cos(rotation.z) - (float)Math.Cos(rotation.x)*(float)Math.Sin(rotation.z),
                (float)Math.Cos(rotation.x)*(float)Math.Sin(rotation.y)*(float)Math.Cos(rotation.z) + (float)Math.Sin(rotation.x)*(float)Math.Sin(rotation.z) },
           {    (float)Math.Cos(rotation.y) * (float)Math.Sin(rotation.z),
                (float)Math.Sin(rotation.x)*(float)Math.Sin(rotation.y)*(float)Math.Sin(rotation.z) + (float)Math.Cos(rotation.x)*(float)Math.Cos(rotation.z),
                (float)Math.Cos(rotation.x)*(float)Math.Sin(rotation.y)*(float)Math.Sin(rotation.z) -  (float)Math.Sin(rotation.x)*(float)Math.Cos(rotation.z) },
           {    -(float)Math.Sin(rotation.y),
                (float)Math.Sin(rotation.x)*(float)Math.Cos(rotation.y),
                (float)Math.Cos(rotation.x)*(float)Math.Cos(rotation.y) }
        });

        return matrixRotation;
    }

    #endregion

}
