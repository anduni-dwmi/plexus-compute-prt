using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

public class PointGenerator : MonoBehaviour
{
    public Mesh defaultMesh;
    public Material material = null;

    //public ComputeShader compute = null;

    public ComputeBuffer computeBuffer;
    private int kernel;

    private struct Constants
    {
        public static readonly string kernelName = "Main";

        public static readonly int COMPUTEBUFFER = Shader.PropertyToID("_computeBuffer");
        public static readonly int VERTEXBUFFER = Shader.PropertyToID("_vertexBuffer");
        public static readonly int VERTEXCOUNT = Shader.PropertyToID("_vertexCount");
    }
    
    public int vertexCount;
    public Vector4[] vertexBuffer;
    public Vector3[] meshBuffer;
    public int[] indexBuffer;

    private void Start()
    {
        if (dimensions.GridCellCount() <= 0) return;

        material = GetComponent<MeshRenderer>().material;

        GenerateMesh();
        InitializeBuffers();
        SetShaderConstants();

        computeBuffer.Release();
    }

    public Vector3Int dimensions = Vector3Int.one;

    void GenerateMesh ()
    {
        vertexCount = dimensions.GridCellCount();
        vertexBuffer = new Vector4[vertexCount];
        meshBuffer = new Vector3[vertexCount];
        indexBuffer = new int[vertexCount];

        PopulateMeshData();
        StoreMeshData();

        Debug.Log(vertexCount);
    }

    private void PopulateMeshData()
    {
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    int index = x * dimensions.z * dimensions.y + y * dimensions.z + z;
                    meshBuffer[index] = new Vector3(x, y, z);
                    vertexBuffer[index] = new Vector4(x, y, z, 1);
                    indexBuffer[index] = index;
                }
            }
        }
    }

    private void StoreMeshData()
    {
        var mesh = new Mesh();
        mesh.name = "mesh";
        mesh.vertices = meshBuffer;
        mesh.SetIndices(indexBuffer, MeshTopology.Points, submesh: 0);
        Bounds bounds = mesh.bounds;
        bounds.size *= 2f;
        mesh.bounds = bounds;
        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    void ProcessMesh (Mesh mesh)
    {
        //vertexCount = mesh.vertexCount;
        //vertices = mesh.vertices;
    }

    private void InitializeBuffers()
    {
        computeBuffer = new ComputeBuffer(vertexCount, 3 * sizeof(float));
        computeBuffer.SetData(meshBuffer);
        //compute.SetBuffer(kernel, Constants.POINTS, pointsBuffer);
    }

    void SetShaderConstants ()
    {
        material.SetBuffer(Constants.COMPUTEBUFFER, computeBuffer);
        material.SetVectorArray(Constants.VERTEXBUFFER, vertexBuffer);
        material.SetInt(Constants.VERTEXCOUNT, vertexCount);
        //Debug.Log(pointsBuffer.count);
        //Debug.Log(vertexCount);
    }

    private void PrepareCompute()
    {
        //kernel = compute.FindKernel(Constants.kernelName);
        InitializeBuffers();
        SetShaderConstants();
    }
}