using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using FancyScrollView.Example09;
using System.Collections;
using Hungw;


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
                        if (voca.containTexture)
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
                    if (IOController.Folder.vocabularies.Keys.Count > 1)
                    {
                        IOController.Folder.vocabularies.Clear();
                        IOController.Folder.texture.Clear();
                        Debug.Log("clear");
                    }
                    Folder.vocabularies[folderName] = JsonConvert.DeserializeObject<Vocabulary[]>(json);
                    foreach (Vocabulary v in Folder.vocabularies[folderName])
                    {
                        string key = folderName + v.vocabularyName;
                        if (v.containTexture)
                        {
                            Folder.texture[key] = GetTexture2D(v.vocabularyName, folderName);
                        }
                        else
                        {
                            Folder.texture[key] = null;
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

        public static Texture2D GetTexture2D(string textureName, string folderName)
        {
            string path = Application.persistentDataPath + "/Image/" + folderName + "/" + textureName + ".jpg";
            try
            {
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
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
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
            string[] fileNames = new string[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                fileNames[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
            }
            Array.Sort(fileNames);
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
                Debug.Log("Đã tạo thư mục mới: " + directoryPath);
            }
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
                        if (voca.containTexture)
                        {
                            Folder.texture.Remove(fileName + voca.vocabularyName);
                        }
                    }
                    Folder.vocabularies.Remove(fileName);
                    Folder.name.Remove(fileName);
                    File.Delete(filePath);
                    Directory.Delete(Application.persistentDataPath + "/Image/" + fileName, true);
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
            FancyScrollView.Example09.Example09.Instance.UpdateScrollViewFromFile(folderName);
            UIManager.Instance.ClickFolder(folderName);
            UIManager.Instance.bug.text = "";
            CurrentFolderName = folderName;
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
        public bool containTexture;
        public override string ToString()
        {
            return $"Vocabulary: {vocabularyName}, Translation: {translation}, Type: {type}, containTexture: {containTexture}";
        }

        public Vocabulary(ItemData item)
        {
            this.vocabularyName = item.vocabularyName;
            this.translation = item.translation;
            this.type = item.type;
            this.containTexture = item.texture != null;
        }
        public Vocabulary()
        {

        }
    }

}


