using UnityEngine;
using System.Collections.Generic;

public interface ICraftable{

    // Dictionary<string, int> GetRequirements();

    Dictionary<string, int> GetRequirements();

    CraftType GetCraftType();

    
}