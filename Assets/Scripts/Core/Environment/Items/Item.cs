using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public int amount;

    public abstract void Spawn();
    public abstract void Collect(); // what happened when collect

    // Update is called once per frame
}
