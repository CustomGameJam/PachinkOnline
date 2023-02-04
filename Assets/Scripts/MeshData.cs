using System;
using UnityEngine;

[Serializable]
public class MeshData
{
    public MeshType type = MeshType.Cube;
    public Vector3Int scale = new(1, 1, 0);
    public Vector3Int offset;
}