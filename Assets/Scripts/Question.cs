using System.Collections.Generic;
using IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HungwNguyen.MUIP;

namespace Hungw
{
    public class Question : MonoBehaviour
    {
        [SerializeField] private Answer answer;
        [SerializeField] private TMPro.TMP_Text questionText, answerTextResult;
        [SerializeField] private RawImage image;
        [SerializeField] private float targetWidth, targetHeight;
        [SerializeField] private Animator QA;
        [SerializeField] private TMP_InputField answerText;
        private IO.Vocabulary[] vocabularies;
        private int vocaSize, currentVocaIndex, correctIndex;
        private Dictionary<int, bool> checkQuestion;
        private string[] answers;
        public string correctAns { get; private set; }
        public bool isCorrect { get; set; }
        private string[] replateChar = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public bool isChoiceMode { get; set; }
        public bool CheckAnswer(int index)
        {
            return index == correctIndex;
        }

        public void UnChooseAnswer()
        {
            answer.UnChooseAnswer(correctIndex);
        }

        public void Init()
        {
            checkQuestion = new Dictionary<int, bool>();
            vocabularies = IOController.Folder.vocabularies[IOController.CurrentFolderName];
            vocaSize = vocabularies.Length;
            currentVocaIndex = 0;
            CreateQuestion();
        }

        public void CheckAnswerInChoiceMode()
        {
            this.isCorrect = answerText.text.ToLower().Equals(correctAns.ToLower());
            if (isCorrect)
            {
                SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_correct);
            }
            else
            {
                SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_wrong);
                answerTextResult.text = correctAns;
            }
        }

        private void CreateCurrentIndex()
        {
            if (checkQuestion.ContainsKey(currentVocaIndex) && checkQuestion.Keys.Count < vocaSize)
            {
                this.currentVocaIndex = (currentVocaIndex + 1) % vocaSize;
                CreateCurrentIndex();
            }
            else if (checkQuestion.Keys.Count == vocaSize)
            {
                checkQuestion.Clear();
            }
            else
            {
                checkQuestion[currentVocaIndex] = true;
            }
        }

        public void CreateQuestion()
        {
            answerTextResult.text = "";
            isChoiceMode = Random.Range(0, 1000000000) % 2 == 0;
            isCorrect = false;
            answers = new string[] { "", "", "", "" };
            this.currentVocaIndex = Random.Range(0, vocaSize);
            CreateCurrentIndex();
            CreateAnswer();
            string key = IOController.CurrentFolderName + this.vocabularies[currentVocaIndex].vocabularyName;
            if (IOController.Folder.texture.ContainsKey(key))
            {
                if (IOController.Folder.texture[key] != null)
                {
                    ResizeImage(IOController.Folder.texture[key]);
                }
            }
            else
            {
                this.image.gameObject.SetActive(false);
            }
            string question = this.vocabularies[currentVocaIndex].translation;
            question = question.Substring(0, 1).ToUpper() + question.Substring(1);
            SetQuestion(question + " là gì ?", answers);
        }


        private int GetWrongIndex()
        {
            int index = Random.Range(0, 4);
            while (answers[index] != "")
            {
                index = (index + 1) % 4;
            }
            return index;
        }

        private int GetWrongVocabularyIndex()
        {
            int index = Random.Range(0, vocaSize);
            while (index == currentVocaIndex)
            {
                index = (index + 1) % vocaSize;
            }
            return index;
        }

        private void CreateChoiceMode()
        {
            correctIndex = Random.Range(0, 4);
            answers[correctIndex] = correctAns;
            string wrongAnswer = vocabularies[GetWrongVocabularyIndex()].vocabularyName;
            answers[GetWrongIndex()] = wrongAnswer;
            answers[GetWrongIndex()] = GetRandomReplateCharacter(wrongAnswer);
            CreateWrongAnswer(GetWrongIndex());
        }

        private void CreateAnswer()
        {
            this.correctAns = vocabularies[currentVocaIndex].vocabularyName;
            if (isChoiceMode)
            {
                CreateChoiceMode();
                QA.Play("Create question");
            }
            else
            {
                QA.Play("Create question 2");
                answerText.text = "";
            }
            
        }

        private void CreateWrongAnswer(int index)
        {
            int type = Random.Range(0, 2);
            switch (type)
            {
                case 0:
                    answers[index] = GetRandomReplateCharacter(this.correctAns);
                    break;
                case 1:
                    answers[index] = GetRandomRedundantCharacter();
                    break;
            }
        }

        private string GetRandomReplateCharacter(string correctAns)
        {
            int randomIndex = Random.Range(1, correctAns.Length), randomCharIndex = Random.Range(0, this.replateChar.Length);
            string changeChar = correctAns[randomIndex].ToString(), token = correctAns;
            string replateChar = this.replateChar[randomCharIndex], result;
            if (replateChar.Equals(changeChar))
            {
                randomCharIndex = (randomCharIndex + 1) % this.replateChar.Length;
            }
            result = token.Remove(randomIndex, 1).Insert(randomIndex, this.replateChar[randomCharIndex]);
            for (int i = 0; i < 4; i++)
            {
                while (answers[i].Equals(result))
                {
                    token = correctAns;
                    randomCharIndex = (randomCharIndex + 1) % this.replateChar.Length;
                    result = token.Remove(randomIndex, 1).Insert(randomIndex, this.replateChar[randomCharIndex]);
                    break;
                }
            }
            return result;
        }

        private string GetRandomRedundantCharacter()
        {
            string token = correctAns, result;
            int randomIndex = Random.Range(1, correctAns.Length), randomCharIndex = Random.Range(0, this.replateChar.Length);

            string redundantChar = this.replateChar[randomCharIndex];
            while (redundantChar == "")
            {
                randomCharIndex = (randomCharIndex + 1) % this.replateChar.Length;
                redundantChar = this.replateChar[randomCharIndex];
            }
            result = token.Insert(randomIndex, redundantChar);
            for (int i = 0; i < 4; i++)
            {
                while (answers[i].Equals(result))
                {
                    randomCharIndex = (randomCharIndex + 1) % this.replateChar.Length;
                    redundantChar = this.replateChar[randomCharIndex];
                    while (redundantChar == "")
                    {
                        randomCharIndex = (randomCharIndex + 1) % this.replateChar.Length;
                        redundantChar = this.replateChar[randomCharIndex];
                    }
                    token = correctAns;
                    result = token.Insert(randomIndex, redundantChar);
                    break;
                }
            }
            return result;
        }

        public void SetQuestion(string question, string[] answers)
        {
            questionText.text = question;
            answer.SetAnswerText(answers);
        }

        private void ResizeImage(Texture2D texture)
        {
            image.texture = texture;
            image.gameObject.SetActive(true);
            float currentWidth = texture.width;
            float currentHeight = texture.height;
            if (currentHeight == currentWidth)
            {
                image.rectTransform.sizeDelta = Vector2.one * targetHeight;
            }
            else if (currentHeight > currentWidth)
            {
                image.rectTransform.sizeDelta = new Vector2(targetHeight / currentHeight * currentWidth, targetHeight);
            }
            else
            {
                image.rectTransform.sizeDelta = new Vector2(targetWidth, targetWidth / currentWidth * currentHeight);
            }
        }

    }
}
