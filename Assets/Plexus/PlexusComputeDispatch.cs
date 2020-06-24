using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlexusComputeDispatch : MonoBehaviour
{
    // compute shader constants
    [BoxGroup("settings")] public ComputeShader compute;
    int kernel;
    int kernel_main;

    Mesh mesh;
    Material material;

    private struct Constants
    {
        public static readonly int VERTEXBUFFER = Shader.PropertyToID("_vertexBuffer");
        public static readonly int INDEXBUFFER = Shader.PropertyToID("_indexBuffer");
        public static readonly int CONNECTIONBUFFER = Shader.PropertyToID("_connectionBuffer");
    }

    // dispatch dimension
    [BoxGroup("settings")] public int buffersize;
    [BoxGroup("settings")] public Vector3Int dispatch = Vector3Int.one;

    [BoxGroup("result")] public Vector3[] vertexResult;
    [BoxGroup("result")] public int[] indexResult;
    [BoxGroup("result")] public int[] connectionsResult;

    ComputeBuffer vertexBuffer;
    ComputeBuffer indexBuffer;
    ComputeBuffer connectionsBuffer;

    void Dispatch()
    {
        vertexResult = new Vector3[buffersize];
        indexResult = new int[buffersize];
        connectionsResult = new int[buffersize];

        kernel = compute.FindKernel("flood");
        kernel_main = compute.FindKernel("main");
        vertexBuffer = new ComputeBuffer(buffersize, sizeof(float)*3);
        indexBuffer = new ComputeBuffer(buffersize, sizeof(int));
        connectionsBuffer = new ComputeBuffer(buffersize, sizeof(int));


        compute.SetBuffer(kernel, Constants.VERTEXBUFFER, vertexBuffer);
        compute.SetBuffer(kernel, Constants.INDEXBUFFER, indexBuffer);
        compute.Dispatch(kernel, dispatch.x, dispatch.y, dispatch.z);


        vertexBuffer.GetData(vertexResult);
        //vertexBuffer.Release();

        indexBuffer.GetData(indexResult);
        indexBuffer.Release();



        compute.SetBuffer(kernel_main, Constants.VERTEXBUFFER, vertexBuffer);
        compute.SetBuffer(kernel_main, Constants.CONNECTIONBUFFER, connectionsBuffer);
        compute.Dispatch(kernel_main, dispatch.x, dispatch.y, dispatch.z);

        connectionsBuffer.GetData(connectionsResult);

        vertexBuffer.Release();
        connectionsBuffer.Release();
    }

    void PrepareCompute ()
    {
        compute.SetInt("_count", buffersize);
        compute.SetInts("_groups", dispatch.x, dispatch.y, dispatch.z);
        compute.SetInts("_threads", 4, 4, 4);
    }

    
    void PrepareShader ()
    {
        material.SetBuffer(Constants.VERTEXBUFFER, vertexBuffer);
        material.SetBuffer(Constants.CONNECTIONBUFFER, connectionsBuffer);
    }

    private void StoreMeshData(Vector3[] vertices, int[] indices)
    {
        var mesh = new Mesh();
        mesh.name = "mesh";
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Points, submesh: 0);
        Bounds bounds = mesh.bounds;
        bounds.size *= 2f;
        mesh.bounds = bounds;
        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    void SetupMeshData(Vector3[] vertices, int[] indices)
    {
        var mesh = new Mesh();
        mesh.name = "mesh";
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Points, submesh: 0);
        Bounds bounds = mesh.bounds;
        bounds.size *= 2f;
        mesh.bounds = bounds;
        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        PrepareCompute();
        PrepareShader();
        Dispatch();
        StoreMeshData(vertexResult, indexResult);
    }

}