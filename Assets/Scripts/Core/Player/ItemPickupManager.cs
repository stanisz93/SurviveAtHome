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
    
    List <SpoonItem> PossibleToPicked; 
    SpoonItem bestOption;
    // Start is called before the first frame update
    void Start()
    {
        PossibleToPicked = new List<SpoonItem>();
        StartCoroutine(CheckPotentialPickups());
    }

    public void AddPotentialObject(SpoonItem spoon)
    {
        PossibleToPicked.Add(spoon);
    }
    public void RemovePotentialObject(SpoonItem spoon)
    {
        if(spoon != null)
            PossibleToPicked.Remove(spoon);
    }

    public void PickItem()
    {
        SpoonItem best = GetBestOption();
        if(best != null)
        {
            RemovePotentialObject(best);
            collectiblePopup.PopUp(best);
            best.RunPickEvent();
        }

    }




    public SpoonItem GetBestOption() {return bestOption;}
    void SetBestOption()
    {
        List <SpoonItem> PreprocessL = new List<SpoonItem>(); // Checking again if objects are directed properly
        foreach(SpoonItem spoon in PossibleToPicked)
        {
            if(spoon.GetRelativeDirection(t_mesh) > 0f)
            PreprocessL.Add(spoon);
        }
        int size = PreprocessL.Count();
        if(size == 0)
            bestOption = null;
        else if(size == 1)
            bestOption = PreprocessL[0];
        else
            bestOption = PreprocessL.Aggregate((i1,i2) => i1.GetRelativeDirection(t_mesh) > i2.GetRelativeDirection(t_mesh) ? i1 : i2);
        if(debugDistance && size > 1)
        {
            Debug.Log($"Best current option: {bestOption}");
            // foreach(SpoonItem spoon in PossibleToPicked)
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
