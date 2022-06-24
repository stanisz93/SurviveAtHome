using UnityEngine;
class MouseUtils
{


    public static Vector3 MousePositonRelativeToCamera(Vector2 mousePos, Camera camera, Transform relativeTo, LayerMask aimLayerMask)
    {
        Ray ray = camera.ScreenPointToRay(mousePos);
        
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