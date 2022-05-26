using UnityEngine;
class TransformUtils
{
    public static Vector3 GetXZDirection(Transform from, Transform to, bool normalized=false)
    {
        Vector3 dir = to.position - from.position;
        dir = new Vector3(dir.x, from.position.y, dir.z);
        if(normalized)
            return dir.normalized;
        else
            return dir;
    }

    public static Vector3 GetXZDirectionWithMargin(Transform from, Transform to, float margin, bool normalized=false)
    {
        Vector3 dir = GetXZDirection(from, to, normalized);
        Vector3 dV = dir.normalized * margin;
        return dir - dir.normalized * margin;
    }
}