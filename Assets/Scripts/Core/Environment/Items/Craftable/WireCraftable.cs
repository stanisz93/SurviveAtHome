using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is a nice way of creating object using Scriptable
// Object for each new component
public class WireCraftable : MonoBehaviour, ICraftable
{
    // Start is called before the first frame update
    public Dictionary<string, int> GetRequirements()
    {
        return new Dictionary<string, int>{{ResourceType.Metal.ToString(), 2}};
    }
   
   public CraftType GetCraftType()
   {
       return CraftType.Wire;
   }
}
