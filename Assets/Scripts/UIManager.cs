using FancyScrollView.Example01;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using IO;
using UnityEngine.SceneManagement;
using HungwNguyen.MUIP;
using UnityEngine.UI;

namespace Hungw
{
    public class UIManager : MonoBehaviour
    {
        [Serializable]
        public class CustomEvent : UnityEvent { }

        #region variables
        [Space(5f), Header("Run when MonoBehaviour call Awake!"), Space(5f)]
        public CustomEvent _customEvent0 = new CustomEvent();
        [Space(5f), Header("Run when change tep 1!"), Space(5f)]
        public CustomEvent _customEvent1 = new CustomEvent();
        [Space(5f), Header("Run when change tep 2!"), Space(5f)]
        public CustomEvent _customEvent2 = new CustomEvent();
        [Space(5f), Header("Run when open a folder!"), Space(5f)]
        public CustomEvent _customEvent3 = new CustomEvent();
        [Space(5f), Header("Run when confirm delete a folder!"), Space(5f)]
        public CustomEvent _customEvent4 = new CustomEvent();
        [Space(5f), Header("Run when click a folder!"), Space(5f)]
        public CustomEvent _customEvent5 = new CustomEvent();

        [SerializeField] private TMP_InputField folderName, folderNameEdit;
        [SerializeField] private List<ButtonManager> buttonSave;
        public TMP_Text bug, notification;
        [SerializeField] private AutoCompleteScript AutoCompleteScript;
        [SerializeField] private NotificationManager saveData;
        [SerializeField] private CanvasGroup canvas;
        [SerializeField] private Slider slider, sliderFX, sliderBG;

        private bool isDelayToSave;

        public static UIManager Instance { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            IOController.Folder.name = IOController.GetAllFileNames();
            AutoCompleteScript.UpdatePromtList(IOController.Folder.name);
            IOController.Folder.vocabularies = new Dictionary<string, IO.Vocabulary[]>();
            IOController.Folder.texture = new Dictionary<string, Texture2D>();
            Instance = this;
            isDelayToSave = false;
            CacheData();
            this._customEvent0.Invoke();
            folderNameEdit.onEndEdit.AddListener(ReNameFolder);
            sliderFX.onValueChanged.AddListener(ChangeFXMusic);
            sliderBG.onValueChanged.AddListener(ChangeBGMusic);
        }
        #endregion

        #region Unity Events

        public void IncreaseScrollView09()
        {
            slider.value++;
        }

        public void DecreaseScrollView09()
        {
            slider.value--;
        }

        private void CacheData()
        {
            int count = IOController.Folder.name.Count;
            while (count > 0)
            {
                if (IOController.Folder.name.Count - count > 1)
                {
                    break;
                }
                count--;
                IOController.ReadData(IOController.Folder.name[count]);
            }
        }

        private void LoadCanvas(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        private void UnLoadCanvas(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public void ChangeBGMusic(float value)
        {
            SoundManager.ChangeVolumeBGMusic(value);
        }

        public void ChangeFXMusic(float value)
        {
            SoundManager.ChangeVolumeFXSound(value);
        }

        public void LoadGameScene()
        {
            if (SceneManager.sceneCount > 1)
            {
                CanvasGroup gameCanvas = GameObject.FindWithTag("Canvas").GetComponent<CanvasGroup>();
                gameCanvas.transform.GetChild(0).gameObject.SetActive(true);
                LoadCanvas(gameCanvas);
            }
            else
            {
                SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
            }
            UnLoadCanvas(canvas);
        }

        public void UnLoadGameScene()
        {
            CanvasGroup gameCanvas = GameObject.FindWithTag("Canvas").GetComponent<CanvasGroup>();
            gameCanvas.transform.GetChild(0).gameObject.SetActive(false);
            LoadCanvas(canvas);
            UnLoadCanvas(gameCanvas);
        }

        #region Folder Manager Methods
        public void OpenFolder()
        {
            _customEvent3.Invoke();
            FancyScrollView.Example09.ScrollView.Instance.ResetScroll();
        }
        public void ClickFolder(string folderName)
        {
            IOController.CurrentFolderName = folderName;
            _customEvent5.Invoke();
            folderNameEdit.text = folderName;
        }

        public void ReNameFolder(String value)
        {
            if (value.Equals(IOController.CurrentFolderName) || CheckSyntax(value))
            {
                return;
            }
            IOController.RenameFolder(IOController.CurrentFolderName, value);
            IOController.CurrentFolderName = value;
            AutoCompleteScript.UpdatePromtList(IOController.Folder.name);
            Example01.Instance.UpdateData();
            PlayerPrefs.DeleteKey(IOController.CurrentFolderName);
        }
        #endregion

        public void DeleteFile()
        {
            APIController.Instance.DeleteData();
            IOController.DeleteFile(IOController.CurrentFolderName);
            AutoCompleteScript.UpdatePromtList(IOController.Folder.name);
            Example01.Instance.UpdateData();
        }

        public void OpenInternetNotification()
        {
            _customEvent4.Invoke();
        }
        public void PlayAudioClick()
        {
            SoundManager.CreatePlayFXSound();
        }

        public void ChangeStep(int value)
        {
            switch (value)
            {
                case 1:
                    if (CheckSyntax(folderName.text))
                    {
                        break;
                    }
                    CheckOverLap(folderName.text);
                    FancyScrollView.Example09.Example09.Instance.StartCreate();
                    IOController.CurrentFolderName = folderName.text;
                    _customEvent1.Invoke();
                    break;
                case 2:
                    SaveFolder(false, () => _customEvent2.Invoke());
                    break;
                case 3:
                    SaveFolder();
                    break;
            }
        }

        // public void DeleteSoundData()
        // {
        //     IOController.DeleteSound(IOController.CurrentFolderName);
        // }

        public void SaveFolder(bool isSaveFromAPI = false, Action afterSaveAction = null)
        {
            if (isDelayToSave)
            {
                return;
            }
            else
            {
                StartCoroutine(DelayToSave());
            }
            string mes = FancyScrollView.Example09.ScrollView.Instance.CheckExeptionAndSave(IOController.CurrentFolderName, isSaveFromAPI);
            if (mes != null)
            {
                bug.text = mes;
                return;
            }
            else
            {
                if (afterSaveAction != null)
                {
                    afterSaveAction.Invoke();
                }
                bug.text = "";
                notification.text = "Chúc mừng ký chủ đã lưu thành công!";
                if (!IOController.Folder.name.Contains(IOController.CurrentFolderName))
                {
                    IOController.Folder.name.Add(IOController.CurrentFolderName);
                    IOController.Folder.name.Sort();
                    AutoCompleteScript.UpdatePromtList(IOController.Folder.name);
                    Example01.Instance.UpdateData();
                }
                saveData.OpenNotification();
            }
        }

        IEnumerator DelayToSave()
        {
            isDelayToSave = true;
            foreach (var button in buttonSave)
            {
                button.SetText("Đã lưu!");
            }
            yield return new WaitForSeconds(1f);
            foreach (var button in buttonSave)
            {
                button.SetText("Lưu");
            }
            isDelayToSave = false;
        }

        public void ClearBug()
        {
            bug.text = "";
        }

        #endregion
        #region Check Methods
        public void CheckOverLap(string value)
        {
            if (IOController.Folder.name.Contains(value))
            {
                for (int i = 1; ; i++)
                {
                    if (!IOController.Folder.name.Contains(value + $"({i})"))
                    {
                        folderName.text = value + $"({i})";
                        break;
                    }
                }
            }
            bug.text = "";
        }
        public bool CheckSyntax(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                bug.text = "Tên thư mục không được bỏ trống!";
                return true;
            }
            else if (value.Contains("\\") || value.Contains("/"))
            {
                bug.text = "Tên thư mục không được chứa ký tự đặc biệt!";
                return true;
            }
            else
            {
                bug.text = "";
            }
            return false;
        }
        #endregion
    }
}
