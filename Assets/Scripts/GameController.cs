using IO;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Hungw
{
    public class GameController : MonoBehaviour
    {

        [SerializeField] private TMP_Text input;
        [SerializeField] private bool isTest = false;

        private void Awake()
        {
            if (isTest)
            {
                IOController.Folder.vocabularies = new Dictionary<string, IO.Vocabulary[]>();
                IOController.Folder.texture = new Dictionary<string, Texture2D>();
                IOController.CurrentFolderName = "Test";
                IOController.ReadData(IOController.CurrentFolderName);
                GameObject.FindWithTag("Finish").gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            string result = "";
            foreach (var v in IOController.Folder.vocabularies[IOController.CurrentFolderName])
            {
                result += v.ToString() + " ";
            }
            input.text = result;
        }

        public void LoadScene()
        {
            UIManager.Instance.UnLoadGameScene();
        }
    }
}
