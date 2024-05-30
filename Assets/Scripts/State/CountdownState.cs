using System.Collections;
using Hungw;
using TMPro;
using UnityEngine;

public class CountdownState : State
{
    private TMP_Text countdownText;
    private int countdownTimer, originalTimer;
    private int count;
    private float timer;
    public CountdownState(TMP_Text countdownText, int countdownTimer)
    {
        this.countdownText = countdownText;
        this.countdownTimer = countdownTimer;
        this.originalTimer = countdownTimer;
        this.count = 0;
    }
    public void ResetCount()
    {
        count = 0;
        this.countdownTimer = originalTimer;
    }

    public override void Exit()
    {
        countdownText.GetComponent<Animator>().Play("disappear");
    }

    public override void Enter()
    {
        this.timer = 0;
        count++;
        int step = count / 8;
        this.countdownTimer = originalTimer - step * 5;
        this.countdownTimer = countdownTimer > 0 ? countdownTimer : 5;
        countdownText.text = $"00:{(countdownTimer > 9 ? countdownTimer : "0" + countdownTimer)}";
        countdownText.GetComponent<Animator>().Play("appear");
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;
        int time = countdownTimer - (int)timer;
        countdownText.text = $"00:{(time > 9 ? time : "0" + time)}";
        if (time <= 0)
        {
            GameController.Instance.isEndLimitTime = true;
            stateMachine.ChangeState(nextState);
        }
    }
}