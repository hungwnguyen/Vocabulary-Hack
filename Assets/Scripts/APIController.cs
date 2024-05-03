using UnityEngine;
using Proyecto26;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using IO;
using HungwAPi;
using System.Net.Http;
using System.Collections;
using HungwNguyen.MUIP;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace Hungw
{
    public class APIController : MonoBehaviour
    {

        #region variables

        [Space(5f), Header("Run when progress start!"), Space(5f)]
        public UnityEvent _OnProgress = new UnityEvent();
        [Space(5f), Header("Run when get data success!"), Space(5f)]
        public UnityEvent _OnGetImageSuccess = new UnityEvent();
        [Space(5f), Header("Run when get data error!"), Space(5f)]
        public UnityEvent _OnGetDataError = new UnityEvent();
        [SerializeField] private string autoCompletePath, autoCompletePath2, googleTranslatePath, googleTranslatePath2;
        [SerializeField] private string soundPathUK, soundPathUS, ServerURL, ServerKey, jsonblobURL;
        [SerializeField] private List<string> soundKeys;
        [SerializeField] private RadialSlider progressSlider;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Toggle voiceToggleMale, voiceToggleFemale;
        /// <summary>
        /// The speed progress of the API controller.
        /// </summary>
        [SerializeField] private float speedProgress;
        [SerializeField] private TMP_InputField blodInputField;
        [SerializeField] private GameObject cellContainer;
        private AutocompleteVocabulary autocompleteVocabulary;
        public static APIController Instance { get; private set; }
        public Dictionary<string, string> currentTranslation, currentType;
        private int currentAPInumber, maxCount;
        private string currentPath, blodId, currentGoogleTranslatePath;
        public bool isChangePath { get; private set; }
        private HungwAPi.Folder folder;
        private int imageCount;
        private string currentVoid;
        #endregion
        private void Awake()
        {
            if (PlayerPrefs.GetInt("Data", 0) == 0)
            {
                IOController.CreateDirectory("Image");
                IOController.CreateDirectory("Sound");
                PlayerPrefs.SetInt("Data", 1);
            }
            //currentAPInumber = PlayerPrefs.GetInt("soundPath", Random.Range(0, soundKeys.Count));
            currentAPInumber = PlayerPrefs.GetInt("soundPath", 0);
            autocompleteVocabulary = new AutocompleteVocabulary();
            autocompleteVocabulary.suggestions = new List<Vocabulary>();
            currentTranslation = new Dictionary<string, string>();
            currentType = new Dictionary<string, string>();
            currentPath = autoCompletePath;
            currentGoogleTranslatePath = googleTranslatePath;
            isChangePath = false;
            this.blodInputField.onEndEdit.AddListener(OnblodInputFieldEndEdit);
            RestClient.Get(Application.persistentDataPath);
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            this.currentVoid = PlayerPrefs.GetString("void", "John");
            if (!currentVoid.Equals("John"))
            {
                voiceToggleFemale.isOn = true;
            }
            else
            {
                voiceToggleFemale.isOn = false;
            }
        }

        private void OnblodInputFieldEndEdit(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            ResetUI();
            blodId = value;
            StartCoroutine(ProgressAnimation(50));

            GetDataJsonBlob(value);

            _OnProgress.Invoke();
        }

        public void CopyText()
        {
            GUIUtility.systemCopyBuffer = PlayerPrefs.GetString(IOController.CurrentFolderName);
        }

        private void ResetUI()
        {
            imageCount = 0;
            progressText.transform.GetChild(0).gameObject.SetActive(false);
            this.progressSlider.SetSliderValue(0);
            this.progressSlider.currentValue = 0;
            progressText.text = "Đang tải dữ liệu vui lòng đợi!";
        }

        public void ShareData()
        {
            _OnProgress.Invoke();
            ResetUI();
            int length = IOController.Folder.vocabularies[IOController.CurrentFolderName].Length;
            StartCoroutine(ProgressAnimation((float)imageCount / length * 100));
            maxCount = 0;
            progressSlider.currentValue = (float)imageCount / maxCount * 100;
            imageCount++;
            bool isUploadImage = false;
            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrEmpty(IOController.Folder.vocabularies[IOController.CurrentFolderName][i].texturePath))
                {
                    isUploadImage = true;
                    maxCount++;
                }
            }
            if (!isUploadImage)
            {
                ShareJson();
                StartCoroutine(ProgressAnimation(100));
            }
            else
            {
                StartCoroutine(ProgressAnimation(1 / maxCount * 100));
            }
            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrEmpty(IOController.Folder.vocabularies[IOController.CurrentFolderName][i].texturePath))
                {
                    UploadImageFile(IOController.Folder.vocabularies[IOController.CurrentFolderName][i].vocabularyName, IOController.CurrentFolderName, i);
                }
            }
        }

        public void SetOnUploadImageComplete(string url, int index)
        {
            IOController.Folder.vocabularies[IOController.CurrentFolderName][index].texturePath = url;
            if (imageCount == maxCount)
            {
                imageCount = 0;
                ShareJson();
            }
            else
            {
                progressSlider.currentValue = (float)imageCount / maxCount * 100;
                imageCount++;
                StartCoroutine(ProgressAnimation((imageCount - 0.02f) / maxCount * 100));
            }

        }

        IEnumerator ProgressAnimation(float targetValue)
        {

            while (progressSlider.currentValue < targetValue)
            {
                progressSlider.currentValue += UnityEngine.Random.Range(0.5f, 1.5f) * Time.deltaTime * speedProgress;
                if (progressSlider.currentValue > targetValue)
                {
                    progressSlider.currentValue = targetValue;
                    progressSlider.SetSliderValue(progressSlider.currentValue);
                    yield break;
                }
                progressSlider.SetSliderValue(progressSlider.currentValue);
                yield return new WaitForEndOfFrame();
            }
        }

        public void CancelUploadImage()
        {
            StopAllCoroutines();
        }

        private void ShareJson()
        {
            UploadDataJsonBlob();
            //blodId = PlayerPrefs.GetString(IOController.CurrentFolderName, "");
            // if (string.IsNullOrEmpty(blodId))
            // {
            //     UploadDataJsonBlob();
            // }
            // else
            // {
            //     UpdateDataJsonBlob(blodId);
            // }
        }

        public void DeleteData()
        {
            blodId = PlayerPrefs.GetString(IOController.CurrentFolderName, "");
            if (!string.IsNullOrEmpty(blodId))
            {
                PlayerPrefs.DeleteKey(IOController.CurrentFolderName);
            }
        }

        private HungwAPi.Folder GetFolder()
        {
            folder = new HungwAPi.Folder();
            folder.folderName = IOController.CurrentFolderName;
            folder.vocabularies = IOController.Folder.vocabularies[IOController.CurrentFolderName];
            return folder;
        }

        public void UploadDataJsonBlob()
        {

            StartCoroutine(UploadJsonBlob(GetFolder()));
        }

        public void GetDataJsonBlob(string blobId)
        {
            StartCoroutine(GetJsonBlob(blobId));
        }

        public void UpdateDataJsonBlob(string blobId)
        {
            StartCoroutine(UpdateJsonBlob(blobId, GetFolder()));
        }

        public void DeleteDataJsonBlob(string blobId)
        {
            StartCoroutine(DeleteJsonBlob(blobId));
        }


        // Method to retrieve a JSON Blob
        private IEnumerator GetJsonBlob(string blobId)
        {
            var request = new UnityWebRequest(jsonblobURL + blobId, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                this.folder = JsonConvert.DeserializeObject<HungwAPi.Folder>(request.downloadHandler.text);
                if (IOController.Folder.name.Contains(folder.folderName))
                {
                    for (int i = 1; ; i++)
                    {
                        if (!IOController.Folder.name.Contains(folder.folderName + $"({i})"))
                        {
                            folder.folderName = folder.folderName + $"({i})";
                            break;
                        }
                    }
                }
                IOController.CurrentFolderName = folder.folderName;
                IOController.Folder.vocabularies[folder.folderName] = folder.vocabularies;
                OnGetJsonBlobComplete();
            }
            else
            {
                _OnGetDataError.Invoke();
                UIManager.Instance.bug.text = "Không tìm thấy dữ liệu!";
                StopAllCoroutines();
            }
        }
        public void OnGetJsonBlobComplete()
        {
            progressSlider.currentValue = 50;
            StartCoroutine(DownloadImages());
        }

        public IEnumerator DownloadImages()
        {
            for (int i = 0; i < folder.vocabularies.Length; i++)
            {
                if (!string.IsNullOrEmpty(folder.vocabularies[i].texturePath))
                {
                    string key = folder.folderName + folder.vocabularies[i].vocabularyName;
                    UnityWebRequest request = UnityWebRequestTexture.GetTexture(folder.vocabularies[i].texturePath);

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                        IOController.Folder.texture.Add(key, texture);
                        progressSlider.currentValue = 50 + 50 * i / folder.vocabularies.Length;
                        StartCoroutine(ProgressAnimation(50 + 50 * (i + 0.98f) / folder.vocabularies.Length));
                    }
                    else
                    {
                        Debug.Log(request.error);
                    }
                }
            }
            FancyScrollView.Example09.Example09.Instance.UpdateScrollViewFromFile(this.folder.folderName);
            progressSlider.SetSliderValue(100);
            progressText.text = "<color=green>" + "Chúc mừng bạn đã tải xong!" + "</color>";
            yield return new WaitForEndOfFrame();
            _OnGetImageSuccess.Invoke();
            UIManager.Instance.SaveFolder(true);
        }

        // Method to update a JSON Blob
        private IEnumerator UpdateJsonBlob(string blobId, object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var request = new UnityWebRequest(jsonblobURL + blobId, "PUT");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                OnUploadJsonBlobComplete();
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        public void OnUploadJsonBlobComplete()
        {
            StopAllCoroutines();
            IOController.SaveData(IOController.CurrentFolderName);
            this.progressSlider.SetSliderValue(100);
            progressText.text = "<color=green>" + PlayerPrefs.GetString(IOController.CurrentFolderName) + "</color>";
            progressText.transform.GetChild(0).gameObject.SetActive(true);
        }

        // Method to delete a JSON Blob
        private IEnumerator DeleteJsonBlob(string blobId)
        {
            var request = new UnityWebRequest(jsonblobURL + blobId, "DELETE");
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Blob deleted successfully");
            }
            else
            {
                Debug.Log(request.error);
            }
        }
        private IEnumerator UploadJsonBlob(object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var request = new UnityWebRequest(jsonblobURL, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.responseCode == 201)
            {
                string url = request.GetResponseHeader("Location");
                string blobId = url.Substring(url.LastIndexOf("/") + 1);
                PlayerPrefs.SetString(IOController.CurrentFolderName, blobId);
                OnUploadJsonBlobComplete();
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        public void UploadImageFile(string imageName, string folderName, int index)
        {
            string key = folderName + imageName;
            string url = IOController.Folder.vocabularies[folderName][index].texturePath;
            if (!string.IsNullOrEmpty(url) && !url.Equals("."))
            {
                SetOnUploadImageComplete(url, index);
                Debug.Log(url);
            }
            else
            {
                StartCoroutine(UploadImageCoroutine(IOController.Folder.texture[key], index));
            }
        }

        IEnumerator UploadImageCoroutine(Texture2D textureToSave, int index)
        {
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    byte[] imageData = textureToSave.EncodeToJPG();

                    content.Add(new StringContent(this.ServerKey), "key");
                    content.Add(new ByteArrayContent(imageData, 0, imageData.Length), "source", "image.jpg");

                    var response = client.PostAsync(this.ServerURL, content);

                    // Wait for the task to complete
                    while (!response.IsCompleted)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    var responseString = response.Result.Content.ReadAsStringAsync();

                    // Wait for the task to complete
                    while (!responseString.IsCompleted)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    Root root = JsonConvert.DeserializeObject<Root>(responseString.Result);
                    // TODO: Handle the response
                    Debug.Log(root.image.url);
                    SetOnUploadImageComplete(root.image.url, index);
                }
            }
        }

        public void SetCurrentPath()
        {
            isChangePath = !isChangePath;
            if (isChangePath)
            {
                currentPath = autoCompletePath2;
                this.currentGoogleTranslatePath = googleTranslatePath2;
            }
            else
            {
                currentPath = autoCompletePath;
                this.currentGoogleTranslatePath = googleTranslatePath;
            }
            for (int i = 0; i < cellContainer.transform.childCount; i++)
            {
                FancyScrollView.Example09.Cell cell = cellContainer.transform.GetChild(i).GetComponent<FancyScrollView.Example09.Cell>();
                if (isChangePath)
                {
                    cell.SetDescription("Nhập Tiếng Việt", "Sửa bản dịch tiếng Anh.");
                }
                else
                {
                    cell.SetDescription("Nhập Tiếng Anh", "Sửa bản dịch tiếng Việt.");
                }
            }
        }

        public void GetTranslatetionFromGoogle(string vocabulary, FancyScrollView.Example09.Cell currentCell)
        {
            if (!currentTranslation.ContainsKey(vocabulary))
            {
                RestClient.Get(currentGoogleTranslatePath + vocabulary).Then(res =>
                {
                    string word = SubString(res.Text, "[[[\"", "\",");
                    currentTranslation.Add(vocabulary, word);
                    currentCell.SetTranslation(word);
                });
            }
            else
            {
                currentCell.SetTranslation(currentTranslation[vocabulary]);
            }
        }

        public void GetVocabularyAutoComplete(string vocabulary, AutoCompleteScript autoCompleteScript, FancyScrollView.Example09.Cell currentCell)
        {
            if (string.IsNullOrEmpty(vocabulary))
            {
                currentCell.SetTranslation("");
                currentCell.SetType("");
                return;
            }
            GetTranslatetionFromGoogle(vocabulary, currentCell);
            RestClient.Get(currentPath + vocabulary).Then(res =>
            {
                autocompleteVocabulary = JsonConvert.DeserializeObject<AutocompleteVocabulary>(res.Text);
                List<string> promtlist = new List<string>();
                foreach (Vocabulary v in autocompleteVocabulary.suggestions)
                {
                    promtlist.Add(v.select);
                    if (!currentType.ContainsKey(v.select))
                    {
                        currentType.Add(v.select, GetTypeVocabulary(v.data));
                    }
                }
                autoCompleteScript.UpdatePromtList(promtlist);
                autoCompleteScript.OnInputChanged();
                if (autocompleteVocabulary.suggestions.Count > 0)
                {
                    UIManager.Instance.ClearBug();
                    string key = autocompleteVocabulary.suggestions[0].select;
                    currentCell.SetType(currentType[key]);
                }
                else
                {
                    currentCell.SetType("");
                }
            })
            .Catch(
                err =>
                {
                    Debug.LogError(err.Message);
                    UIManager.Instance.bug.text = "Không có kết nối mạng!";
                }
            )
            ;
        }

        private string SubString(string input, string start, string end)
        {
            string output = "";
            int startIndex = input.IndexOf(start) + start.Length;
            int endIndex = input.IndexOf(end);
            if (endIndex <= startIndex)
            {
                return output;
            }
            else
            {
                output = input.Substring(startIndex, endIndex - startIndex);
            }
            return output;
        }

        private string GetTypeVocabulary(string input)
        {
            return SubString(input, "</span><span class=\"fr hl\" >", "<img src=");
        }

        private string GetTranslation(string input)
        {
            return SubString(input, "></div><p>", "</p></a>");
        }

        public void SetVoid()
        {
            if (voiceToggleFemale.isOn)
            {
                SetVoid("Mary");
            }
            else
            {
                SetVoid("John");
            }
        }

        private void SetVoid(string name)
        {
            this.currentVoid = name;
            PlayerPrefs.SetString("void", name);
        }

        public void GetSound(string word)
        {

            if (SoundManager.CreatePlayFXSound(word))
                return;

            string api = this.currentVoid.Equals("John") ? soundPathUK : soundPathUS;

            Sound sound = new Sound();

            RestClient.Get(api + word).Then(res =>
            {
                sound = JsonConvert.DeserializeObject<Sound>(res.Text);
                if (sound.error == 0)
                {
                    var fileUrl = sound.data;
                    var fileType = AudioType.MPEG;

                    RestClient.Get(new RequestHelper
                    {
                        Uri = fileUrl,
                        DownloadHandler = new DownloadHandlerAudioClip(fileUrl, fileType)
                    }).Then(res =>
                    {
                        UIManager.Instance.ClearBug();
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(res.Request);
                        if (clip.length == 0)
                        {
                            UIManager.Instance.bug.text = "Không tìm thấy âm thanh";
                        }
                        else
                        {
                            clip.name = word;
                            SoundManager.CreatePlayFXSound(clip);
                        }
                        //string filePath = Path.Combine(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Music", audio.clip.name);
                        //File.WriteAllBytes(filePath, res.Request.downloadHandler.data);
                    }).Catch(err =>
                    {
                        Debug.Log(err.Message);
                    }
                    );
                }
                else
                {
                    UIManager.Instance.bug.text = "Không tìm thấy âm thanh";
                }
            }).Catch(err =>
            {
                Debug.Log(err.Message);
                UIManager.Instance.bug.text = "Không có kết nối mạng!";
            }
            );
        }

        // public void GetSound(string word)
        // {

        //     if (SoundManager.CreatePlayFXSound(word))
        //         return;

        //     string fileUrl = $"https://api.voicerss.org/?key={this.soundKeys[currentAPInumber]}&hl=en-us&v={this.currentVoid}&c=MP3&src=" + word;
        //     var fileType = AudioType.MPEG;

        //     RestClient.Get(new RequestHelper
        //     {
        //         Uri = fileUrl,
        //         DownloadHandler = new DownloadHandlerAudioClip(fileUrl, fileType)
        //     }).Then(res =>
        //     {
        //         UIManager.Instance.ClearBug();
        //         AudioClip clip = DownloadHandlerAudioClip.GetContent(res.Request);
        //         if (clip.length == 0)
        //         {
        //             currentAPInumber = (currentAPInumber + 1) % soundKeys.Count;
        //             PlayerPrefs.SetInt("soundPath", currentAPInumber);
        //             GetSound(word);
        //         }
        //         else
        //         {
        //             clip.name = word;
        //             SoundManager.CreatePlayFXSound(clip);
        //         }
        //         //string filePath = Path.Combine(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Music", audio.clip.name);
        //         //File.WriteAllBytes(filePath, res.Request.downloadHandler.data);
        //     }).Catch(err =>
        //     {
        //         Debug.Log(err.Message);
        //         UIManager.Instance.bug.text = "Không có kết nối mạng";
        //     }
        //     );
        // }
    }

    public class AutocompleteVocabulary
    {
        public string query;
        public List<Vocabulary> suggestions;
    }

    public class Vocabulary
    {
        public string select;
        public string data;
    }

    public class Sound
    {
        public int error;
        public string data;
    }

}
namespace HungwAPi
{
    public class Folder
    {
        public string folderName;
        public IO.Vocabulary[] vocabularies;
    }

    public class JSONBlob
    {
        public string Location { get; set; }
    }
    public class Success
    {
        public string message { get; set; }
        public int code { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }

    public class Root
    {
        public int status_code { get; set; }
        public Success success { get; set; }
        public Image image { get; set; }
        public string status_txt { get; set; }
    }
}