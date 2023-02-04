using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class MeshUtil
{
    private const int Height = 2;

    public static void Render(SerializedProperty dataList)
    {
        var (mesh, renderer) = StartRender(dataList.serializedObject);
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int[][]>();
        var materials = new List<Material>();

        float x = 0, y = 0, z = 0;
        for (var i = 0; i < dataList.arraySize; i++)
        {
            var data = dataList.GetArrayElementAtIndex(i);

            var scale = data.FindPropertyRelative("scale").vector3IntValue;
            var offset = data.FindPropertyRelative("offset").vector3IntValue;

            var renderData = Render(
                new Vector2(x + offset.x, scale.x + x + offset.x),
                new Vector2(y + offset.y, scale.y + y + offset.y),
                new Vector2(z + offset.z, scale.z + z + offset.z)
            );

            if (renderData == null)
            {
                continue;
            }

            vertices.AddRange(renderData.Item1);
            uvs.AddRange(renderData.Item2);
            tris.Add(renderData.Item3.Select(s => s.Select(e => e + i * 12).ToArray()).ToArray());
            materials.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/t1.mat"));
            materials.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/t2.mat"));
            materials.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/t3.mat"));

            x += Mathf.Sqrt(Mathf.Pow(scale.x, 2) - Mathf.Pow(scale.z, 2));
            z += scale.z;
        }

        renderer.sharedMaterials = materials.ToArray();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.subMeshCount = tris.Count * 3;

        for (var i = 0; i < tris.Count; i++)
        {
            mesh.SetTriangles(tris[i][0], i * 3);
            mesh.SetTriangles(tris[i][1], i * 3 + 1);
            mesh.SetTriangles(tris[i][2], i * 3 + 2);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static Tuple<Mesh, MeshRenderer> StartRender(SerializedObject serializedObject)
    {
        var mesher = (serializedObject.targetObject as Mesher)!;
        var filter = mesher.GetComponent<MeshFilter>();
        var mesh = filter.sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
            filter.sharedMesh = mesh;
        }

        mesh.Clear();

        var renderer = mesher.GetComponent<MeshRenderer>();
        return Tuple.Create(mesh, renderer);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static Tuple<Vector3[], Vector2[], int[][]> Render(Vector2 xs, Vector2 ys, Vector2 zs)
    {
        var x = xs.y - xs.x;
        var y = ys.y - ys.x;
        var z = zs.y - zs.x;

        var tris = new[]
        {
            // Side
            new[]
            {
                0, 2, 1,
                2, 3, 1,
            },
            // Top
            new[]
            {
                4, 6, 5,
                6, 7, 5,
            },
            // Front
            new[]
            {
                9, 10, 8,
                9, 11, 10
            }
        };

        var xDiff = Mathf.Sqrt(Mathf.Pow(xs.x - xs.y, 2) - Mathf.Pow(zs.x - zs.y, 2));
        var vertices = new Vector3[]
        {
            // Side
            new(xs.x, -zs.x, ys.x),
            new(xs.x + xDiff, -zs.y, ys.x),
            new(xs.x, Height - zs.x, ys.x),
            new(xs.x + xDiff, Height - zs.y, ys.x),

            // Top
            new(xs.x, Height - zs.x, ys.x),
            new(xs.x + xDiff, Height - zs.y, ys.x),
            new(xs.x, Height - zs.x, ys.y),
            new(xs.x + xDiff, Height - zs.y, ys.y),

            // Front
            new(xs.x + xDiff, -zs.y, ys.x),
            new(xs.x + xDiff, Height - zs.y, ys.x),
            new(xs.x + xDiff, -zs.y, ys.y),
            new(xs.x + xDiff, Height - zs.y, ys.y),
        };

        var uvs = new Vector2[]
        {
            // Side
            new(0, 0),
            new(x, 0),
            new(0, Height),
            new(x, Height),

            // Top
            new(0, 0),
            new(x, 0),
            new(0, y),
            new(x, y),

            // Front
            new(0, 0),
            new(Height, 0),
            new(0, y),
            new(Height, y),
        };

        return Tuple.Create(vertices, uvs, tris);
    }
}