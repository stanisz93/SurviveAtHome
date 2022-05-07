using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    // Start is called before the first frame update
    public float yAxisOffset = 2f;

    private CableGenerator hookRef;
    private HingeJoint joint;
    void Start()
    {
        joint = GetComponent<HingeJoint>();
        joint.connectedAnchor = new Vector3(0f, -yAxisOffset, 0f);
        hookRef = GetComponentInParent<CableGenerator>();
    }

    // Update is called once per frame
    public float GetYOffset()
    {
        return yAxisOffset;
    }

    private void OnTriggerEnter(Collider other) {

        string plrTag = "Player";
        string opponentTag = "Opponent";
        string triggerTag = other.transform.tag; 
        if ((triggerTag == plrTag) || (triggerTag == opponentTag))
        {
            if(triggerTag == opponentTag && !hookRef.IsBroken())
            {
                Opponent opp = other.gameObject.GetComponent<Opponent>();
                if(opp.GetAgentSpeed() > 0.5f)
                {
                    other.gameObject.GetComponent<Opponent>().Fall();
                    Debug.Log($"Collide with {other.gameObject}");
                    Destroy(gameObject);
                    hookRef.RemovePoint(this);
                }
                
                
            }
        }
    }

}
