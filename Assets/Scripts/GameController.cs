using IO;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Hungw
{
    public class GameController : MonoBehaviour
    {
        public bool isTest = false;
        public Question question;
        public static GameController Instance;
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
        void OnEnable()
        {
            question.Init();
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
