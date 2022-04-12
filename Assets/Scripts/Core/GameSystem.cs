using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class GameSystem : MonoBehaviour {
 
    public static GameSystem Instance { get; private set; }
    public bool playerDebug = false;
    
    public bool opponentDebug = false;
    public bool taskManagerDebug = false;
    public bool opponentDamageDebug = false;

    public enum aliasingOption {None = 0, x2 = 2, x4 = 4, x8 = 8}; 
    public aliasingOption antliAliasing;
    
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

       private void Update() {
         QualitySettings.antiAliasing = (int)antliAliasing;
     }

     
     }

  