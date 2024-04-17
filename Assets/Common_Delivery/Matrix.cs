using System;
using System.Runtime.CompilerServices;

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

        return new Vector3C(m.data[0,0] * a.x + m.data[0,1] * a.y + m.data[0,2] * a.z,
            m.data[1, 0] * a.x + m.data[1, 1] * a.y + m.data[1, 2] * a.z,
            m.data[2, 0] * a.x + m.data[2, 1] * a.y + m.data[2, 2] * a.z)
    }

    public static MatrixC operator *(MatrixC m1, MatrixC m2)
    {
        float[] results = new float[m1.size * m1.size];
        for (int i = 0; i < m1.size; i++)
        {
            for (int j = 0; j < m1.size; j++)
            {
                int suma = 0;
                for (int k = 0; k < m1.size; k++)
                {
                    suma += (int)(m1.data[i * m1.size + k] * m2.data[k * m1.size + j]);
                }
                results[i * m1.size + j] = suma;
            }
        }

        return new MatrixC(results);
    }
    #endregion

    #region METHODS
    public void Rotate()
    {

    }

    public float Determintate3x3(MatrixC m)
    {
        if (m.size != 3)
        {
            return 0;
        }
        float determinate;

        determinate = (m.data[0] * m.data[4] * m.data[8] + m.data[1] * m.data[5] * m.data[6] + m.data[2] * m.data[3] * m.data[7]) - 
            (m.data[2] * m.data[4] * m.data[6] + m.data[0] * m.data[5] * m.data[7] + m.data[1] * m.data[3] * m.data[8]);

        return determinate;
    }

    public MatrixC Transposed(MatrixC m)
    {
        MatrixC matrix = new MatrixC(m.data);

        m.data[0] = matrix.data[0];
        m.data[1] = matrix.data[3];
        m.data[2] = matrix.data[6];

        m.data[3] = matrix.data[1];
        m.data[4] = matrix.data[4];
        m.data[5] = matrix.data[7];

        m.data[6] = matrix.data[2];
        m.data[7] = matrix.data[5];
        m.data[8] = matrix.data[8];

        return m;
    }

    #endregion

    #region FUNCTIONS
    public MatrixC Inverse()
    {


        return new MatrixC(data);
    }
    #endregion

}
