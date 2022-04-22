using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemPickupManager : MonoBehaviour
{
// This class should store encounter items and
// Highlity one that is directed the most toward player.

    public Transform t_mesh; // needed for proper direction
    public CollectiblePopup collectiblePopup;
    private Character character;
    public bool debugDistance=false;
    
    List <ICollectible> PossibleToPicked; 
    Transform bestOption;
    // Start is called before the first frame update
    void Start()
    {
        PossibleToPicked = new List<ICollectible>();
        StartCoroutine(CheckPotentialPickups());
    }

    public void AddPotentialObject(ICollectible item)
    {
        PossibleToPicked.Add(item);
    }
    public void RemovePotentialObject(ICollectible item)
    {
        if(item != null)
            PossibleToPicked.Remove(item);
    }

    public void PickICollectible()
    {
        if(bestOption != null)
        {
            ICollectible best = (ICollectible)bestOption.gameObject.GetComponent<ICollectible>();
            RemovePotentialObject(best);
            collectiblePopup.PopUp(best);
            best.Collect();
        }

    }




    public Transform GetBestOption() {return bestOption;}

    public float GetRelativeDirection(Transform objTransform)
    {
        Vector3 dir = (objTransform.position - t_mesh.position).normalized;
        
        float score = Vector3.Dot(dir, t_mesh.forward);   
        return score;
    }
    void SetBestOption()
    {
        List <Transform> PreprocessL = new List<Transform>(); // Checking again if objects are directed properly
        foreach(ICollectible item in PossibleToPicked)
        {
            if(GetRelativeDirection(item.transform) > 0f)
                PreprocessL.Add(item.transform);
        }
        int size = PreprocessL.Count();
        if(size == 0)
            bestOption = null;
        else if(size == 1)
            bestOption = PreprocessL[0];
        else
            bestOption = PreprocessL.Aggregate((i1,i2) => GetRelativeDirection(i1) > GetRelativeDirection(i2) ? i1 : i2);
        if(debugDistance && size > 1)
        {
            Debug.Log($"Best current option: {bestOption}");
            // foreach(SpoonICollectible spoon in PossibleToPicked)
            // {
            //     Debug.Log($"Spoon with coordinates {spoon.transform.position} has score {spoon.GetRelativeDirection(t_mesh)}");
            // }
        }
    }


    IEnumerator CheckPotentialPickups()
    {
        while(true)
        {
            SetBestOption();
            yield return new WaitForSeconds(0.2f);
        }
    }

        
    



    // Update is called once per frame
    
}
