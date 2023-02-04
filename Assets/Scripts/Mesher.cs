using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mesher : MonoBehaviour
{
    public MeshData[] data = { new() };

    public int current;

    void Start()
    {
        var colors = new[]
        {
            Color.blue,
            Color.black,
            Color.green,
            Color.magenta,
            Color.yellow,
            Color.cyan,
        };
        var material = Resources.Load<Material>("Materials/Dop");
        foreach (var obj in GameObject.FindGameObjectsWithTag("Dop"))
        {
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshRenderer.material.color = colors[Random.Range(0, colors.Length)];
        }
    }
}