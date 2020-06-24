// # include file for compute shader

int boundsSize(uint3 bounds)
{
    return bounds.x * bounds.y * bounds.z;
}
int calculateId(uint3 id, uint3 bounds)
{
    return bounds.x * bounds.y * id.z + bounds.x * id.y + id.x;
}

bool boundsValid(uint3 id, uint3 bounds)
{
    if (id.x > bounds.x ||
        id.y > bounds.y ||
        id.z > bounds.z)
        return false;
    return true;
}

int calculateThreadId (uint3 threadId, uint3 threadBounds, uint3 groupId, uint3 groupBounds)
{
    // check for bound errors and return default [-1] on invalid bounds
    if (!boundsValid(threadId, threadBounds) || !boundsValid(groupId, groupBounds))
        return -1;
    
    // calculate thread id
    return calculateId(threadId, threadBounds) + boundsSize(threadBounds) * calculateId(groupId, groupBounds);
}


float3 random(int index)
{
    return frac(sin(index * float3(54998.1518, 21114.1262, 55223.9101))) * 8.8436;
}