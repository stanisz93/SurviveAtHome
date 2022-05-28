using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ExplorationSquare : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    
    public Vector3 GetARandomTreePos()
    {

        Mesh planeMesh = gameObject.GetComponent<MeshFilter>().mesh;
        Bounds bounds = planeMesh.bounds;
        float x = gameObject.transform.position.x;
        float z = gameObject.transform.position.z;

        float shiftX = gameObject.transform.localScale.x * bounds.size.x * 0.5f;
        float shiftZ = gameObject.transform.localScale.z * bounds.size.z * 0.5f;
        // Debug.Log($"MinX {minX}, {minZ}");
        Vector3 newVec = new Vector3(Random.Range(x - shiftX, x + shiftX),
                                    0f,
                                    Random.Range(z - shiftZ, z + shiftZ));
    return newVec;
    }

    private void OnDestroy() {
        Destroy(GetComponent<OpponentUtils>()); 
    }

}
