using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using FancyScrollView.Example09;
using System.Collections;
using Hungw;
using System.Security.Cryptography;

namespace IO
{
    public static class IOController
    {
        public static Folder Folder = new Folder();
        public static string CurrentFolderName = string.Empty;
        public static void RenameFolder(string oldName, string newName)
        {
            string oldPath = GetFilePath(oldName);
            string newPath = GetFilePath(newName); ;
            if (File.Exists(oldPath))
            {
                try
                {
                    File.Move(oldPath, newPath);
                    Directory.Move(Application.persistentDataPath + "/Image/" + oldName, Application.persistentDataPath + "/Image/" + newName);
                    Folder.vocabularies[newName] = Folder.vocabularies[oldName];
                    foreach (Vocabulary voca in Folder.vocabularies[newName])
                    {
                        string key = newName + voca.vocabularyName,
                                oldKey = oldName + voca.vocabularyName;
                        if (!string.IsNullOrEmpty(voca.texturePath))
                        {
                            Folder.texture[key] = Folder.texture[oldKey];
                        }
                        else
                        {
                            Folder.texture[key] = null;
                        }
                    }
                    Folder.vocabularies.Remove(oldName);
                    Folder.name.Remove(oldName);
                    Folder.texture.Remove(oldName);
                    Folder.name.Add(newName);
                    Folder.name.Sort();
                    CurrentFolderName = newName;
                    Debug.Log("Đã đổi tên thư mục: " + oldName + " thành " + newName);
                }
                catch (IOException e)
                {
                    Debug.Log("Lỗi khi đổi tên thư mục: " + e.Message);
                }
            }
            else
            {
                Debug.Log("Không tìm thấy tệp tin để đổi tên: " + oldName);
            }
        }

        public static string GetAudioPath(string textureName)
        {
            string path = Application.persistentDataPath + "/Sound/" + CurrentFolderName + "/" + textureName + ".mp3";
            return File.Exists(path) ? path : string.Empty;
        }
        public static void SaveAudioFile(byte[] audioData, string textureName)
        {
            CreateDirectory(CurrentFolderName, "/Sound/");
            string path = Application.persistentDataPath + "/Sound/" + CurrentFolderName + "/" + textureName + ".mp3";
            try
            {
                File.WriteAllBytes(path, audioData);
            }
            catch (IOException e)
            {
                Debug.Log("Error while saving audio file: " + e.Message);
            }
        }
        public static void SaveData(string folderName)
        {
            string path = GetFilePath(folderName);
            string json = JsonConvert.SerializeObject(Folder.vocabularies[folderName], Formatting.Indented);
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(json);
                }
            }
            catch (IOException e)
            {
                Debug.Log(e.Message);
            }
        }

        public static void ReadData(string folderName)
        {
            string path = GetFilePath(folderName);
            string json = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    json = reader.ReadToEnd();
                    if (IOController.Folder.vocabularies.Keys.Count > 6)
                    {
                        IOController.Folder.vocabularies.Clear();
                        IOController.Folder.texture.Clear();
                        Debug.Log("clear");
                    }
                    Folder.vocabularies[folderName] = JsonConvert.DeserializeObject<Vocabulary[]>(json);
                    foreach (Vocabulary v in Folder.vocabularies[folderName])
                    {
                        string key = folderName + v.vocabularyName;
                        if (!string.IsNullOrEmpty(v.texturePath) && CheckFolderImage(v.vocabularyName, folderName))
                        {
                            Folder.texture[key] = GetTexture2D(v.vocabularyName, folderName);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.Log(e.Message);
                UIManager.Instance.bug.text = "Tên thư mục không tồn tại!";
            }
        }

        private static bool CheckFolderImage(string textureName, string folderName)
        {
            string path = Application.persistentDataPath + "/Image/" + folderName + "/" + textureName + ".jpg";
            return File.Exists(path);
        }

        public static Texture2D GetTexture2D(string textureName, string folderName)
        {
            string path = Application.persistentDataPath + "/Image/" + folderName + "/" + textureName + ".jpg";
            byte[] imageData = File.ReadAllBytes(path); // Đọc dữ liệu từ tệp
            Texture2D texture = new Texture2D(2, 2); // Tạo một Texture2D mới

            // Load ảnh từ mảng byte
            if (texture.LoadImage(imageData))
            {
                return texture;
            }
            else
            {
                Debug.LogError("Failed to load texture from file: " + path);
                return null;
            }
        }

        public static void SaveTexture(Texture2D textureToSave, string folderName, string textureName)
        {
            string filePath = Application.persistentDataPath + "/Image/" + folderName + "/" + textureName + ".jpg";
            byte[] bytes = textureToSave.EncodeToJPG(66);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            string key = folderName + textureName;
            Folder.texture[key] = texture;
            File.WriteAllBytes(filePath, bytes);
        }


        public static string GetFilePath(string folderName)
        {
            return Application.persistentDataPath + Path.AltDirectorySeparatorChar + folderName + ".json";
        }

        public static List<string> GetAllFileNames()
        {
            string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
            List<string> fileNames = new List<string>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (Path.GetExtension(filePaths[i]) != ".json") continue;
                fileNames.Add(Path.GetFileNameWithoutExtension(filePaths[i]));
            }
            fileNames.Sort();
            return fileNames.ToList<string>();
        }

        public static void CreateDirectory(string folderName, string directoryPath = "")
        {
            if (directoryPath == "")
            {
                directoryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + folderName;
            }
            else
            {
                directoryPath = Application.persistentDataPath + directoryPath + folderName;
            }
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static void DeleteSound(string fileName)
        {
            if (Directory.Exists(Application.persistentDataPath + "/Sound/" + fileName))
                Directory.Delete(Application.persistentDataPath + "/Sound/" + fileName, true);
        }
        public static void DeleteFile(string fileName)
        {
            string filePath = GetFilePath(fileName); ;
            if (File.Exists(filePath))
            {
                try
                {
                    foreach (Vocabulary voca in Folder.vocabularies[fileName])
                    {
                        if (!string.IsNullOrEmpty(voca.texturePath))
                        {
                            Folder.texture.Remove(fileName + voca.vocabularyName);
                        }
                    }
                    Folder.vocabularies.Remove(fileName);
                    Folder.name.Remove(fileName);
                    File.Delete(filePath);
                    Directory.Delete(Application.persistentDataPath + "/Image/" + fileName, true);
                    DeleteSound(fileName);
                    Debug.Log("Đã xóa tệp tin: " + fileName);
                }
                catch (IOException e)
                {
                    Debug.Log("Lỗi khi xóa tệp tin: " + e.Message);
                }
            }
            else
            {
                Debug.Log("Không tìm thấy tệp tin để xóa: " + fileName);
            }
        }

        public static IEnumerator OpenFolderRecent(string folderName)
        {
            if (!Folder.vocabularies.ContainsKey(folderName))
            {
                IOController.ReadData(folderName);
                yield return new WaitForEndOfFrame();
            }
            if (!Folder.vocabularies.ContainsKey(folderName)) yield break;
            UIManager.Instance.bug.text = "";
            CurrentFolderName = folderName;
            UIManager.Instance.ClickFolder(folderName);
            foreach (Vocabulary voca in Folder.vocabularies[folderName])
            {
                if (!string.IsNullOrEmpty(voca.texturePath) && voca.texturePath != ".")
                {
                    if (!CheckFolderImage(voca.vocabularyName, folderName))
                    {
                        APIController.Instance.OnGetJsonBlobComplete(true);
                        yield break;
                    }
                }
            }
            Example09.Instance.UpdateScrollViewFromFile(folderName);
        }
    }

    public class Folder
    {
        public List<string> name;
        public Dictionary<string, Vocabulary[]> vocabularies;
        public Dictionary<string, Texture2D> texture;
    }

    public class Vocabulary
    {
        public string vocabularyName;
        public string translation;
        public string type;
        public string texturePath;
        public override string ToString()
        {
            return $"Vocabulary: {vocabularyName}, Translation: {translation}, Type: {type}, texturePath: {texturePath}";
        }

        public Vocabulary(ItemData item, string texturePathCurrent = "")
        {
            this.vocabularyName = item.vocabularyName;
            this.translation = item.translation;
            this.type = item.type;
            this.texturePath = item.texture == null ? "" : string.IsNullOrEmpty(texturePathCurrent) ? "." :
            item.isChangeTexture ? "." : texturePathCurrent;
        }
        public Vocabulary()
        {

        }
    }

}


