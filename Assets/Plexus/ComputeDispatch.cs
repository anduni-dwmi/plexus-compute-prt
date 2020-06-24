using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeDispatch : MonoBehaviour
{
    public ComputeShader compute;
    ComputeBuffer computeBuffer;
    int kernel;
    
    public int threadgroup_x = 1;
    public int threadgroup_y = 1;
    public int threadgroup_z = 1;

    public int[] idResult;

    void Dispatch ()
    {
        computeBuffer = new ComputeBuffer(128, sizeof(int));
        kernel = compute.FindKernel("main");

        compute.SetBuffer(kernel, "idResult", computeBuffer);
        compute.Dispatch(kernel, threadgroup_x, threadgroup_y, threadgroup_z);

        idResult = new int[128];
        computeBuffer.GetData(idResult);

        computeBuffer.Release();
    }

    // get (int) required group count
    // calculate groups for dispatch

    // send blob size to compute so for thread end ignore

    private void Start()
    {
        Dispatch();
    }
}