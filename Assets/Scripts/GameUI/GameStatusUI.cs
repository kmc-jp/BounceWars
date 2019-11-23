using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusUI : MonoBehaviour
{
    public static readonly Color allyColor = Color.green;
    public static readonly Color enemyColor = Color.red;

    [SerializeField]
    private Text GameTimeText = default;

    [SerializeField]
    private Text Player1UnitText = default;
    [SerializeField]
    private Text Player2UnitText = default;

    [SerializeField]
    private Image Player1UnitImage = default;
    [SerializeField]
    private Image Player2UnitImage = default;

    /// <param name="player">1 or 2</param>
    public void UpdatePlayerUnitText(int player, int unitNum)
    {
        if (player == 1)
            UpdatePlayer1UnitText(unitNum);
        else if (player == 2)
            UpdatePlayer2UnitText(unitNum);
    }

    public void UpdatePlayer1UnitText(int unitNum)
    {
        Player1UnitText.text = unitNum.ToString();
    }

    public void UpdatePlayer2UnitText(int unitNum)
    {
        Player2UnitText.text = unitNum.ToString();
    }

    public void UpdateGameTimeText(int time)
    {
        int minute = time / 60;
        int second = time % 60;
        GameTimeText.text = string.Format("{0}:{1:00}", minute, second);
    }

    public void UpdateGameTimeText(float time)
    {
        UpdateGameTimeText(Mathf.FloorToInt(time));
    }

    public void SetAlly(int player)
    {
        if (player == 1)
            SetAlly1();
        else if (player == 2)
            SetAlly2();
    }

    public void SetAlly1()
    {
        Player1UnitImage.color = allyColor;
        Player2UnitImage.color = enemyColor;
    }

    public void SetAlly2()
    {
        Player2UnitImage.color = allyColor;
        Player1UnitImage.color = enemyColor;
    }
}
