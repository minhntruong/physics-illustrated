using System;

namespace PhysicsIllustrated.Library.Physics.Mathematics;

public class MatMN
{
    public MatMN(int m, int n)
    {
        if (m <= 0 || n <= 0)
            throw new ArgumentException("Matrix dimensions must be greater than zero.");

        M = m;
        N = n;

        _rows = new VectorN[m];
        for (int i = 0; i < m; i++)
        {
            _rows[i] = new VectorN(n);
        }
    }

    public MatMN(MatMN mat)
    {
        M = mat.M;
        N = mat.N;

        _rows = new VectorN[M];

        for (int i = 0; i < M; i++)
        {
            _rows[i] = new VectorN(mat._rows[i]);
        }
    }

    private VectorN[] _rows;

    //==========================================================================

    public int M { get; private set; }

    public int N { get; private set; }

    //==========================================================================

    public float this[int row, int col]
    {
        get
        {
            if (row < 0 || row >= M)
                throw new IndexOutOfRangeException("Row index is out of range.");
            if (col < 0 || col >= N)
                throw new IndexOutOfRangeException("Column index is out of range.");
    
            return _rows[row][col];
        }
        set
        {
            if (row < 0 || row >= M)
                throw new IndexOutOfRangeException("Row index is out of range.");
            if (col < 0 || col >= N)
                throw new IndexOutOfRangeException("Column index is out of range.");
            
            _rows[row][col] = value;
        }
    }

    public VectorN this[int row]
    {
        get
        {
            if (row < 0 || row >= M)
                throw new IndexOutOfRangeException("Row index is out of range.");
            
            return _rows[row];
        }
        set
        {
            if (row < 0 || row >= M)
                throw new IndexOutOfRangeException("Row index is out of range.");
            
            _rows[row] = value;
        }
    }

    public void Zero()
    {
        for (int i = 0; i < M; i++)
        {
            _rows[i].Zero();
        }
    }

    public MatMN Transpose()
    {
        MatMN transposed = new MatMN(N, M);

        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                transposed[j, i] = _rows[i][j];
            }
        }

        return transposed;
    }

    public override string ToString()
    {
        if (_rows == null || _rows.Length == 0)
            return "[]";

        var rowStrings = new string[M];
        for (int i = 0; i < M; i++)
        {
            rowStrings[i] = _rows[i].ToString();
        }
        return "[\n " + string.Join(",\n ", rowStrings) + "\n]";
    }

    //==========================================================================

    // Matrix to matrix multiplication
    public static MatMN operator * (MatMN a, MatMN b)
    {
        if (a.N != b.M)
            throw new ArgumentException("Matrix dimensions are not compatible for multiplication.");

        var transposedB = b.Transpose();

        var result = new MatMN(a.M, b.N);

        for (int i = 0; i < a.M; i++)
        {
            for (int j = 0; j < b.N; j++)
            {
                result[i, j] = a._rows[i].Dot(ref transposedB._rows[j]);
            }
        }

        return result;
    }

    // Matrix to vector multiplication
    public static VectorN operator * (MatMN mat, VectorN vec)
    {
        if (mat.N != vec.N)
            throw new ArgumentException("Matrix columns must match vector size.");

        VectorN result = new VectorN(mat.M);

        for (int i = 0; i < mat.M; i++)
        {
            result[i] = mat._rows[i].Dot(ref vec);
        }
        return result;
    }

    public static VectorN SolveGaussSeidel(MatMN a, VectorN b)
    {
        int n = b.N;

        var x = new VectorN(n);
        
        // Iterate N times
        for (int iterations = 0; iterations < n; iterations++)
        {
            for (int i = 0; i < n; i++)
            {
                if (a[i, i] != 0.0f)
                {
                    x[i] += (b[i] / a[i, i]) - (a[i].Dot(ref x) / a[i, i]);
                }
            }
        }

        return x;
    }
}
