using System;

namespace ShowPhysics.Library.Physics.Math;

public class VectorN
{
    public VectorN()
    {
        _data = new float[0];
        N = 0;
    }

    public VectorN(int size)
    {
        if (size <= 0)
            throw new ArgumentException("Size must be greater than zero.", nameof(size));

        _data = new float[size];
        N = size;
    }
    
    public VectorN(VectorN v)
    {
        N = v.N;

        _data = new float[v._data.Length];

        Array.Copy(v._data, _data, v._data.Length);
    }

    private float[] _data;

    //==========================================================================

    public int N { get; private set; }

    public float this[int index]
    {
        get
        {
            if (index < 0 || index >= _data.Length)
                throw new IndexOutOfRangeException("Index is out of range.");

            return _data[index];
        }
        set
        {
            if (index < 0 || index >= _data.Length)
                throw new IndexOutOfRangeException("Index is out of range.");

            _data[index] = value;
        }
    }

    public void Zero()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = 0f;
        }
    }

    public float Dot(ref VectorN other)
    {
        if (_data.Length != other._data.Length)
            throw new ArgumentException("Vectors must be of the same size.");
     
        float result = 0f;
        
        for (int i = 0; i < _data.Length; i++)
        {
            result += _data[i] * other._data[i];
        }
        
        return result;
    }

    public void CopyFrom(VectorN other)
    {
        _data = new float[other._data.Length];
        N = other.N;

        Array.Copy(other._data, _data, other._data.Length);
    }

    
    public override string ToString()
    {
        if (_data == null || _data.Length == 0)
            return "[]";

        return "[" + string.Join(", ", _data) + "]";
    }    

    //==========================================================================

    public static VectorN operator +(VectorN a, VectorN b)
    {
        if (a._data.Length != b._data.Length)
            throw new ArgumentException("Vectors must be of the same size.");

        var result = new VectorN(a._data.Length);
        
        for (int i = 0; i < a._data.Length; i++)
        {
            result._data[i] = a._data[i] + b._data[i];
        }

        return result;
    }

    public static VectorN operator -(VectorN a, VectorN b)
    {
        if (a._data.Length != b._data.Length)
            throw new ArgumentException("Vectors must be of the same size.");

        var result = new VectorN(a._data.Length);

        for (int i = 0; i < a._data.Length; i++)
        {
            result._data[i] = a._data[i] - b._data[i];
        }

        return result;
    }

    public static VectorN operator *(VectorN a, float scalar)
    {
        var result = new VectorN(a._data.Length);
        for (int i = 0; i < a._data.Length; i++)
        {
            result._data[i] = a._data[i] * scalar;
        }
        return result;
    }

}
