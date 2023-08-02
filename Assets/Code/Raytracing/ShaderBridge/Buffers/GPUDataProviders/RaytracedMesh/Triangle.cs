using UnityEngine;

public readonly struct Triangle
{
    public readonly Vector3 A;
    public readonly Vector3 B;
    public readonly Vector3 C;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        A = a;
        B = b;
        C = c;
    }

    public static int GetSize()
    {
        return 12 + 12 + 12;
    }
}