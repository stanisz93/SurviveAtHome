using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
public class OpponentSpawnManager : MonoBehaviour {
    


public Transform SpawnerPoint;



public int maxAliveOpponents = 10;

public float timeBetweenNextCreation = 2f;
public float populationCheckFrequency = 0.5f;

public GameObject pfOpponent;

public int currentCount = 0;

public Scores scores;

public Text waveText;




private void Start() {
    StartCoroutine(ControlPopulation());
    waveText.text = "Wave 1";
    Sequence sequence = DOTween.Sequence();
    sequence.Join(waveText.transform.DOScale(new Vector3(3f, 3f, 1f), 0.3f));
    sequence.Append(waveText.DOFade(0.0f, 0.1f));
    sequence.SetLoops(7, LoopType.Yoyo);

}


public void ReduceCount(Opponent opponent)
{
    currentCount -= 1;
}


// public RunWaveCount()
// {
//     PoopUpTextTween
// }



private IEnumerator ControlPopulation() {

    while(true)
    {
        if(currentCount < maxAliveOpponents)
        {
            GameObject opponentObj = Instantiate(pfOpponent, SpawnerPoint);
            Opponent opponent = opponentObj.GetComponent<Opponent>();
            opponent.OnKill += ReduceCount;
            opponent.OnKill += scores.UpdateScore;
            currentCount += 1;
            yield return new WaitForSeconds(timeBetweenNextCreation);
        }
        else
            yield return new WaitForSeconds(populationCheckFrequency);
    }

    }
}