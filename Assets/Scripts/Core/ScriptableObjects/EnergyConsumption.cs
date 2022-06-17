using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnergyConsumptionSetting")]
public class EnergyConsumption : ScriptableObject
{
   ///LATER I WOULD LIKE TO MATCH ITS VALUES WITH each Action Type 
   ///In
        ///Maybe for now let for running to be 0

    // Update is called once per frame
    public int knifeAttack = 15;
    public int kickAttack = 30;
    public int stickAttack = 20;

    public int kickWhileVault = 30;
    public int killWhenFaint = 30;

    public int vaultOverBox = 20;
    
    public int slide = 15;

    public int climbWall = 30;

    public int throwEnergy = 10;

    public int pushDoor = 25;

    public int runningPerFrame = 0;

    public int walkEnergyRecovery = 5;
}
