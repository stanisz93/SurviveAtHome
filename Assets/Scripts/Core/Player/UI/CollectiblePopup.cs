using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CollectiblePopup : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cam;
    [SerializeField]
    private GameObject pfCollectPopUp;
    private GameObject collectible;
    public void PopUp(SpoonItem spoon)
    {
        Vector3 v = spoon.transform.position;
        collectible = Instantiate(pfCollectPopUp, v, Quaternion.identity);
        ResourceConfig rConfig = collectible.GetComponent<ResourceConfig>();
        
        rConfig.Setup(spoon.GetAmount(), spoon.GetResourceType().ToString());
        collectible.transform.LookAt(cam.forward + collectible.transform.position);

        // collectible.transform.LookAt(cam.transform);
    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // if(collectible != null)
        // transform.LookAt(collectible.transform.position + cam.forward);
        
    }
}
