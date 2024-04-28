using UnityEngine;
using Proyecto26;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using IO;
using HungwAPi;
using System.Net.Http;
using System.Collections;
using System.IO;

namespace Hungw
{
    public class APIController : MonoBehaviour
    {
        [SerializeField] private string autoCompletePath, autoCompletePath2;
        [SerializeField] private string soundPathUK, soundPathUS, ServerURL, ServerKey, jsonblobURL;
        private AutocompleteVocabulary autocompleteVocabulary;
        public static APIController Instance { get; private set; }
        public Dictionary<string, string[]> currentTranslations;
        private int currentAPInumber;
        private string currentPath;
        private bool isChangePath;
        private string result;

        private void Awake()
        {
            if (PlayerPrefs.GetInt("Image", 0) == 0)
            {
                IOController.CreateDirectory("Image");
                PlayerPrefs.SetInt("Image", 1);
            }
            currentAPInumber = PlayerPrefs.GetInt("soundPath", 0);
            autocompleteVocabulary = new AutocompleteVocabulary();
            autocompleteVocabulary.suggestions = new List<Vocabulary>();
            currentTranslations = new Dictionary<string, string[]>();
            currentPath = autoCompletePath;
            isChangePath = false;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UploadJsonBlob()
        {
            string json = JsonConvert.SerializeObject(IOController.Folder.vocabularies[IOController.CurrentFolderName], Formatting.Indented);
            RestClient.Post(jsonblobURL, json).Then(res =>
            {
                Debug.Log(res.Text);
            }).Catch(err =>
            {
                Debug.Log(err.Message);
            });
        }

        public void UploadImageFile(string imageName, string folderName)
        {
            string key = folderName + imageName;
            StartCoroutine(UploadImageCoroutine(IOController.Folder.texture[key]));
        }

        IEnumerator UploadImageCoroutine(Texture2D textureToSave)
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
                }
            }
        }

        public void SetCurrentPath()
        {
            isChangePath = !isChangePath;
            if (isChangePath)
            {
                currentPath = autoCompletePath2;
            }
            else
            {
                currentPath = autoCompletePath;
            }
        }

        public void GetVocabularyAutoComplete(string vocabulary, AutoCompleteScript autoCompleteScript, FancyScrollView.Example09.Cell currentCell)
        {
            APIController.Instance.currentTranslations.Clear();
            RestClient.Get(currentPath + vocabulary).Then(res =>
            {
                autocompleteVocabulary = JsonConvert.DeserializeObject<AutocompleteVocabulary>(res.Text);
                List<string> promtlist = new List<string>();
                foreach (Vocabulary v in autocompleteVocabulary.suggestions)
                {
                    promtlist.Add(v.select);
                    if (!currentTranslations.ContainsKey(v.select))
                    {
                        currentTranslations.Add(v.select, new string[] { GetTypeVocabulary(v.data), GetTranslation(v.data) });
                    }
                }
                autoCompleteScript.UpdatePromtList(promtlist);
                autoCompleteScript.OnInputChanged();
                if (autocompleteVocabulary.suggestions.Count > 0)
                {
                    UIManager.Instance.ClearBug();
                    string key = autocompleteVocabulary.suggestions[0].select;
                    currentCell.SetTranslateAndType(currentTranslations[key][1], currentTranslations[key][0]);
                }
                else
                {
                    currentCell.SetTranslateAndType("", "");
                }

            })
            // .Catch(
            //     err =>
            //     {
            //         Debug.LogError(err.Message);
            //         UIManager.Instance.bug.text = "Không có kết nối mạng!";
            //     }
            // )
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

        public void GetSound(string word)
        {

            if (this.currentAPInumber == PlayerPrefs.GetInt("soundPath", 0))
            {
                if (SoundManager.CreatePlayFXSound(word))
                    return;
            }
            else
            {
                this.currentAPInumber = PlayerPrefs.GetInt("soundPath", 0);
            }

            string api = currentAPInumber == 0 ? soundPathUK : soundPathUS;

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