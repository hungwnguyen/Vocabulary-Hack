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
        // Start is called before the first frame update

        public void SetAnswerText(string[] answers)
        {
            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].text = choosenAnswer[i] + ". " + answers[i];
            }
        }

        public void SetAnswerColor(int index, bool isCorrect)
        {
            if (!GameController.Instance.isTest)
            {
                APIController.Instance.GetSound(question.correctAns);
            }
            if (isCorrect)
            {
                answerTexts[index].color = correctColor;
            }
            else
            {
                answerTexts[index].color = wrongColor;
            }
        }

        public void ResetAnswerColor()
        {
            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].color = normalColor;
            }
        }

        private IEnumerator ChangeQuestion()
        {
            yield return new WaitForSeconds(1);
            ResetAnswerColor();
            question.CreateQuestion();
            stopTouch.SetActive(false);
        }

        public void ChooseAnswer(int index)
        {
            for (int i = 0; i < answerTexts.Count; i++)
            {
                bool isCorrect = question.CheckAnswer(i);
                if (i == index)
                {
                    SetAnswerColor(i, isCorrect);
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
            stopTouch.SetActive(true);
            StartCoroutine(ChangeQuestion());
        }
    }
}
