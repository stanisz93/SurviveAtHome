using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 public class GameSystem : MonoBehaviour {
 
    public static GameSystem Instance { get; private set; }
    public bool playerDebug = false;
    
    public bool opponentDebug = false;
    public bool taskManagerDebug = false;
    public bool opponentDamageDebug = false;
    public List<GameObject> toDisable;
    private void Awake() {
        if (Instance != null) {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        foreach(GameObject g in toDisable)
        {
            g.GetComponent<MeshRenderer>().enabled = false;
        }
    }
     
     }
