using System.Collections.Generic;
using Hungw;
using UnityEngine;

public class EndState : State
{
    private Question question;
    private List<GameObject> bloods;
    private Animator endAnimator;
    private int count;

    public EndState(Question question, List<GameObject> bloods, Animator endAnimator)
    {
        this.question = question;
        this.bloods = bloods;
        this.endAnimator = endAnimator;
        count = bloods.Count;
    }

    public void Reset()
    {
        count = bloods.Count;
        foreach (var blood in bloods)
        {
            blood.SetActive(true);
        }
    }

    public void DegreasCount()
    {
        if (count > 0)
        {
            bloods[count - 1].SetActive(false);
            count--;
        }
    }

    public override void Enter()
    {
        if (question.isCorrect)
        {
            question.CreateQuestion();
        }
        else if (count > 0)
        {
            if (GameController.Instance.isEndLimitTime)
            {
                GameController.Instance.isEndLimitTime = false;
                if (count > 1)
                {
                    question.UnChooseAnswer();
                    SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_wrong);
                }
                else
                {
                    DegreasCount();
                    endAnimator.Play("appear");
                    SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_win);
                }
            }
            else
            {
                question.CreateQuestion();
            }
        }
        else
        {
            endAnimator.Play("appear");
            SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_win);
        }
    }
}