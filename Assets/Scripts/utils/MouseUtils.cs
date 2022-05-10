using UnityEngine;
class MouseUtils
{


    public static Vector3 MousePositon(Camera camera, Transform relativeTo, LayerMask aimLayerMask)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayerMask))
         {
             var modPoint = new Vector3(hit.point.x, relativeTo.position.y, hit.point.z);
             var _direction = hit.point - relativeTo.position;
             _direction.y = 0f;
             _direction.Normalize();
            return _direction;
         }

       
        return relativeTo.TransformDirection(Vector3.forward);
    }

    
}