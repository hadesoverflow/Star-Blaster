using _GAME.Scripts;
using DenkKits.UIManager.Scripts.Base;
using DenkKits.UIManager.Scripts.UIPopup;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePopupParam
{
    public int score;
    public bool isNewHighScore;
}

public class EndGamePopup : UIPopup
{
    [SerializeField] private TextMeshProUGUI userScore;
    [SerializeField] private TextMeshProUGUI newHighScoreEf;

    protected override void OnShowing()
    {
        base.OnShowing();
        var param = (EndGamePopupParam)Parameter;
        userScore.text = param.score.ToString();

        if (param.isNewHighScore)
        {
            newHighScoreEf.gameObject.SetActive(true);
            newHighScoreEf.DOKill();
            newHighScoreEf.color = Color.yellow; // Đặt màu khởi đầu
            newHighScoreEf
                .DOColor(Color.white, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            newHighScoreEf.gameObject.SetActive(false);
            newHighScoreEf.DOKill();
        }
    }


    public void OnClickExit()
    {
        Hide();
        UIManager.Instance.ShowTransition(() =>
        {
            SceneManager.LoadScene(GameConstants.SceneMain);
        });
    }

    public void OnClickPlayAgain()
    {
        Hide();
        UIManager.Instance.ShowTransition(() =>
        {
            SceneManager.LoadScene(GameConstants.SceneGame);
        });
    }
}