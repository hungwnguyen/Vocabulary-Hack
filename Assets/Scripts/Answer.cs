using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hungw
{
    public class Answer : MonoBehaviour
    {

        [SerializeField] private List<TMP_Text> answerTexts;
        [SerializeField] private Color32 correctColor, wrongColor, normalColor;
        [SerializeField] private Question question;
        [SerializeField] private GameObject stopTouch;
        private string[] choosenAnswer = new string[] { "A", "B", "C", "D" };
        private bool isCorrect;

        public void SetAnswerText(string[] answers)
        {
            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].text = choosenAnswer[i] + ". " + answers[i];
            }
        }

        public void SetAnswerColor(int index, bool isCorrect)
        { 
            if (isCorrect)
            {
                answerTexts[index].color = correctColor;
            }
            else
            {
                answerTexts[index].color = wrongColor;
            }
        }

        public void UnChooseAnswer(int correctIndex)
        {
            if (question.isChoiceMode)
            {
                answerTexts[correctIndex].color = wrongColor;
            }
            stopTouch.SetActive(true);
            StartCoroutine(ChangeQuestion(true));
        }

        public void ResetAnswerColor()
        {
            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].color = normalColor;
            }
        }

        private IEnumerator ChangeQuestion(bool isLimitTime = false)
        {
            if (!GameController.Instance.isTest)
            {
                APIController.Instance.GetSound(question.correctAns);
            }
            if (!question.isCorrect)
            {
                GameController.Instance.endState.DegreasCount();
            }
            else
            {
                GameController.Instance.UpdateScore(100);
            }
            yield return new WaitForSeconds(1);
            if (question.isChoiceMode)
            {
                ResetAnswerColor();
            }
            if (isLimitTime)
            {
                question.CreateQuestion();
            }
            else
            {
                GameController.Instance.ChangeEndState();
            }
            stopTouch.SetActive(false);
        }

        public void ChooseAnswer()
        {
            if (GameController.Instance.isEndLimitTime)
            {
                return;
            }
            question.CheckAnswerInChoiceMode();
            stopTouch.SetActive(true);
            StartCoroutine(ChangeQuestion());
        }

        public void ChooseAnswer(int index)
        {
            if (GameController.Instance.isEndLimitTime)
            {
                return;
            }
            GameController.Instance.ChangeIdleState();
            for (int i = 0; i < answerTexts.Count; i++)
            {
                bool isCorrect = question.CheckAnswer(i);
                if (i == index)
                {
                    SetAnswerColor(i, isCorrect);
                    this.isCorrect = isCorrect;
                    if (isCorrect)
                    {
                        SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_correct);
                    }
                    else
                    {
                        SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_wrong);
                    }
                }
                else if (isCorrect)
                {
                    SetAnswerColor(i, true);
                }
            }
            question.isCorrect = this.isCorrect;
            stopTouch.SetActive(true);
            StartCoroutine(ChangeQuestion());
        }
    }
}
