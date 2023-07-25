using UnityEngine;

public class MeshBuilder
{
    public static Mesh CreateQuad(float width, float height)
    {
        return new Mesh()
        {
            vertices = new Vector3[]
            {
                new(-width / 2, -height / 2, 0),
                new(width / 2, -height / 2, 0),
                new(-width / 2, height / 2, 0),
                new(width / 2, height / 2, 0)
            },
                
            triangles = new[]
            {
                0, 2, 1,
                2, 3, 1
            },
                
            uv = new Vector2[]
            {
                new(0, 0),
                new(1, 0),
                new(0, 1),
                new(1, 1)
            },
                
            normals = new[]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            }
        };
    }
}