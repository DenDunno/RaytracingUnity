using Unity.Collections;
using UnityEngine;

public abstract class Buffer<T> where T : struct
{
    protected Buffer(int count, int stride)
    {
        Container = new ComputeBuffer(count, stride, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
    }

    public ComputeBuffer Container { get; }

    public void Map(int size)
    {
        NativeArray<T> buffer = Container.BeginWrite<T>(0, size);
        
        OnMap(ref buffer);
        
        Container.EndWrite<T>(size);
    }

    protected abstract void OnMap(ref NativeArray<T> buffer);

    ~Buffer() // unity scene reloading. Have no control over this
    {
        Container.Release();
    }
}