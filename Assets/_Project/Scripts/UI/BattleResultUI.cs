using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleResultUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private TMP_Text winnerText;

    private void Awake()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void ShowPlayerWin()
    {
        ShowResult(
            "BATTLE OVER",
            "PLAYER DRAGON WINS!"
        );
    }

    public void ShowEnemyWin()
    {
        ShowResult(
            "BATTLE OVER",
            "ENEMY DRAGON WINS!"
        );
    }

    public void ShowDraw()
    {
        ShowResult(
            "BATTLE OVER",
            "DRAW!"
        );
    }

    private void ShowResult(
        string title,
        string winner
    )
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultTitle != null)
        {
            resultTitle.text = title;
        }

        if (winnerText != null)
        {
            winnerText.text = winner;
        }
    }

    public void RestartBattle()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}