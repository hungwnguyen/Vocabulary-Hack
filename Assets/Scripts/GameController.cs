using IO;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Hungw
{
    public class GameController : MonoBehaviour
    {
        private FiniteStateMachine fsm;
        public EndState endState { get; private set; }
        private CountdownState countdownState;
        private IdleState idleState;
        public bool isTest = false;
        public Question question;
        [SerializeField] private EventHandler onQuestionFinished, onScoreFinishedChanged;
        public static GameController Instance;
        [SerializeField] private TMP_Text countdownText, scoreText, scoreTextHelper, scoreEndText, hightScoreEndText;
        [SerializeField] private int countdownTimer;
        [SerializeField] private UnityEvent OnStartCreateQA;
        [SerializeField] private List<GameObject> bloods;
        [SerializeField] private Animator endAnimator, scoreAnimator;
        private int score;
        public bool isEndLimitTime { get; set; }

        public void UpdateScore(int score = 0, bool isReset = false)
        {
            if (isReset)
            {
                this.score = 0;
                scoreTextHelper.text = this.score + " <color=yellow>$</color>";
                scoreText.text = this.score + " <color=yellow>$</color>";
            }
            else
            {
                this.score += score;
            }
            scoreText.text = this.score > 999999 ? 999999 + "+" : this.score + " <color=yellow>$</color>";
            scoreAnimator.Play("Transition");
            scoreEndText.text = this.score.ToString();
            if (PlayerPrefs.GetInt("HighScore") < this.score)
            {
                PlayerPrefs.SetInt("HighScore", this.score);
                hightScoreEndText.text = this.score.ToString();
            }
        }
        private void ResetScoreText()
        {
            scoreTextHelper.text = this.score + " <color=yellow>$</color>";
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            this.score = 0;
            isEndLimitTime = false;
            fsm = new FiniteStateMachine();
            countdownState = new CountdownState(countdownText, countdownTimer);
            endState = new EndState(question, bloods, endAnimator);
            idleState = new IdleState();
            fsm.Initialization(idleState);
            countdownState.Initialization(fsm, endState);
            onQuestionFinished.OnAnimationFinished += ChangeCountdownState;
            onScoreFinishedChanged.OnAnimationFinished += ResetScoreText;
#if UNITY_EDITOR
            if (isTest)
            {
                IOController.Folder.vocabularies = new Dictionary<string, IO.Vocabulary[]>();
                IOController.Folder.texture = new Dictionary<string, Texture2D>();
                IOController.CurrentFolderName = "Test";
                IOController.ReadData(IOController.CurrentFolderName);
            }
#endif
        }

        public void ResetState()
        {
            countdownState.ResetCount();
            UpdateScore(0, true);
            endState.Reset();
            question.Init();
        }

        public void ChangeIdleState()
        {
            fsm.ChangeState(idleState);
        }

        public void ChangeEndState()
        {
            fsm.ChangeState(endState);
        }

        private void ChangeCountdownState()
        {
            fsm.ChangeState(countdownState);
        }
        private void Update()
        {
            fsm.CurrentState.LogicUpdate();
        }
        void OnEnable()
        {
            hightScoreEndText.text = PlayerPrefs.GetInt("HighScore").ToString();
            question.Init();
            OnStartCreateQA.Invoke();
        }
        public void LoadScene()
        {
            UIManager.Instance.UnLoadGameScene();
            UIManager.Instance.bug.text = "";
        }

        public void PlayClickSound()
        {
            SoundManager.CreatePlayFXSound();
        }

        public void PlayClickSubmitSound()
        {
            SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.aud_submit);
        }
    }
}
