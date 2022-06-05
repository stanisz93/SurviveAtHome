using UnityEngine;
using UnityEngine.UI;
public class Scores : MonoBehaviour {
    

    string scoreText = "Total Kills:";
    public int currentKills = 0;
    public Text score;

    private void Awake() {
        score.text = $"{scoreText} {currentKills}";
    }

    public void UpdateScore(Opponent opponent)
    {
        currentKills += 1;
        score.text = $"{scoreText} {currentKills}";
        DoTweenUtils.PoopUpTextTween(score, Color.red);
    }

}