using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BestCandidateManager: MonoBehaviour
{
    // Start is called before the first frame update
    public Transform t_mesh; // needed for proper direction
    private Character character;
    public bool debugDistance=false;

    List <Transform> PossibleToPicked; 
    private Transform bestOption;
    // Start is called before the first frame update
    void Awake()
    {
        PossibleToPicked = new List<Transform>();
        StartCoroutine(CheckPotentialOptions());
    }

    public void AddPotentialObject(Transform item)
    {
        PossibleToPicked.Add(item);
    }
    public void RemovePotentialObject(Transform item)
    {
        if(item != null)
            PossibleToPicked.Remove(item);
    }

    public Transform GetBestOption()
    {
        if(bestOption != null)
            return bestOption;
        else 
            return null;
    }


    public float GetRelativeDirection(Transform objTransform)
    {
        Vector3 dir = (objTransform.position - t_mesh.position).normalized;
        
        float score = Vector3.Dot(dir, t_mesh.forward);   
        return score;
    }

    void SetBestOption()
    {
        List <Transform> PreprocessL = new List<Transform>(); // Checking again if objects are directed properly
        foreach(Transform item in PossibleToPicked)
        {
            if(GetRelativeDirection(item) > 0f)
                PreprocessL.Add(item);
        }
        int size = PreprocessL.Count;
        if(size == 0)
            bestOption = null;
        else if(size == 1)
            bestOption = PreprocessL[0];
        else
            bestOption = PreprocessL.Aggregate((i1,i2) => GetRelativeDirection(i1) > GetRelativeDirection(i2) ? i1 : i2);
        if(debugDistance && size > 1)
        {
            Debug.Log($"Best current option: {bestOption}");
        }
    }
    
    IEnumerator CheckPotentialOptions()
    {
        while(true)
        {
            SetBestOption();
            yield return new WaitForSeconds(0.2f);
        }
    }
    

}
