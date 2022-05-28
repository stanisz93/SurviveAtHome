using UnityEngine;
using UnityEngine.AI;

public class OpponentUtils : MonoBehaviour
{

    public GameObject looker;

    public ExplorationSquare explorationSquare;



    public Vector3 GetMousePosition()
    {   Vector3 result = new Vector3(0, 0, 0);
        RaycastHit hit;   
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
            result = hit.point;   
        }
        return result;
        
    }



    
    public bool RandomPoint(Vector3 randomPoint, out Vector3 result, int trials=30)
    {   
        for (int i = 0; i < trials; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    public Vector3 FindRandomDestination()
    {   
        Vector3 destination;
        Vector3 random_position = explorationSquare.GetARandomTreePos();
        looker.transform.position = random_position;
        if (GameSystem.Instance.opponentDebug) Debug.Log($"Random values {random_position}");
        bool found_path = RandomPoint(random_position, out destination);
        if(!found_path)
            {if(GameSystem.Instance.opponentDebug) Debug.Log("Path haven't founded!");}
        return destination;
    }


}



