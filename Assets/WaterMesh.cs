using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterMesh : MonoBehaviour
{

    private MeshFilter mesh;
    public int xSize, ySize;
    [SerializeField] private Vector3[] vertex;
    [SerializeField] private Vector2[] vertexPosition;
    private int[] triangles;
    [SerializeField] private float amplitude;
    [SerializeField] private float waveLength;
    [SerializeField] private float fazeSpeed;
    [SerializeField] private float lerpSpeed;
    [SerializeField] int multiplier;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>();
        mesh.mesh = new Mesh();
        makeMeshes();
        i = Mathf.CeilToInt(vertex.Length/ 2);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMesh(vertex, triangles);

        // changeVertexY();
        
    }

    private void changeVertexY()
    {
        if (i >= vertex.Length) { i = Mathf.FloorToInt(vertex.Length/ 2); }
        vertex[i].y = Mathf.Lerp(vertexPosition[i].y, sumOfSins(multiplier, vertexPosition[i]), lerpSpeed);
        i++;
    }

    private float sumOfSins(int amount, Vector2 position)
    {

        float frequency = 2 / waveLength;
        float fazeConstant = fazeSpeed * frequency; 

        float sum = 0;
        for (int i = 0; i < amount; i++)
        {
            sum += amplitude * Mathf.Sin((position.y * frequency) + (Time.deltaTime * fazeConstant));
        }
        return sum;
    }


    void makeMeshes()
    {
        vertex = new Vector3[(xSize + 1) * (ySize + 1)];
        vertexPosition = new Vector2[vertex.Length];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertex[i] = new Vector2(x, y);
                vertexPosition[i] = vertex[i];
                i++;
            }
        }

        triangles = new int[(xSize*ySize) * 6];

        int vertexOffset = 0;
        int trigOffset = 0;

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[trigOffset + 0] = vertexOffset + 0 + y;
                triangles[trigOffset + 1] = vertexOffset + xSize + 1 + y;
                triangles[trigOffset + 2] = vertexOffset + 1 + y;
                triangles[trigOffset + 3] = vertexOffset + 1 + y;
                triangles[trigOffset + 4] = vertexOffset + xSize + 1 + y;
                triangles[trigOffset + 5] = vertexOffset + xSize + 2 + y;

                vertexOffset++;
                trigOffset += 6;
            }
            // yield return new WaitForSeconds(0.1f);


            // int[] triangler = new int[3];
            // triangler[0] = 0;
            // triangler[1] = 1;
        }


        // UpdateMesh(vertex, triangles);
        

    }

    private void UpdateMesh(Vector3[] vertices, int[] triangles)
    {
        mesh.mesh.Clear();
        mesh.mesh.vertices = vertices;
        mesh.mesh.triangles = triangles;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < vertex.Length; i++)
        {
            Gizmos.DrawSphere(vertex[i], 0.1f);
        }
    }

}
