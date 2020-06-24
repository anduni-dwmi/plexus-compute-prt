using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlexusComputeDispatch : MonoBehaviour
{
    // compute shader constants
    public ComputeShader compute;
    ComputeBuffer computeBuffer;
    int kernel;

    private struct Constants
    {
        public static readonly int VERTEXBUFFER = Shader.PropertyToID("_vertexBuffer");
        public static readonly int CONNECTIONBUFFER = Shader.PropertyToID("_connectionBuffer");
    }

    // dispatch dimension
    public Vector3Int dispatch = Vector3Int.one;

    ComputeBuffer vertexBuffer;
    ComputeBuffer connectionBuffer;

    public int buffersize;

    void Dispatch()
    {
        computeBuffer = new ComputeBuffer(buffersize, sizeof(int));
        kernel = compute.FindKernel("main");

        compute.SetBuffer(kernel, Constants.VERTEXBUFFER, vertexBuffer);
        compute.SetBuffer(kernel, Constants.CONNECTIONBUFFER, connectionBuffer);

        compute.Dispatch(kernel, dispatch.x, dispatch.y, dispatch.z);


        computeBuffer.Release();
    }
}