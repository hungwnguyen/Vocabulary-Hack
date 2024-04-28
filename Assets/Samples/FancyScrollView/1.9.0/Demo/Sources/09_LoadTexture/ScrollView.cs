/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using IO;

namespace FancyScrollView.Example09
{
    class ScrollView : FancyScrollView<ItemData>
    {
        [SerializeField] Scroller scroller = default;
        [SerializeField] GameObject cellPrefab = default;

        protected override GameObject CellPrefab => cellPrefab;
        public static ScrollView Instance { get; private set; }
        private float timer;
        protected override void Initialize()
        {
            base.Initialize();
            scroller.OnValueChanged(UpdatePosition);
            this.timer = Time.time;
            Instance = this;
        }

        public void UpdateData(IList<ItemData> items)
        {
            UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        public string RemoveExtraSpaces(string input)
        {
            input = input.Trim();
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");

            return input;
        }

        public void SetTexture(int index, Texture2D target) => ItemsSource[index].texture = target;

        public void SetVocabulary(int index, string vocabulary) => ItemsSource[index].vocabularyName = RemoveExtraSpaces(vocabulary);

        public void SetTranslation(int index, string translation) => ItemsSource[index].translation = RemoveExtraSpaces(translation);

        public void SetType(int index, string type) => ItemsSource[index].type = RemoveExtraSpaces(type);

        public ItemData[] GetItemsSource()
        {
            return ItemsSource.ToArray();
        }

        public string CheckExeptionAndSave(string folderName)
        {
            ItemData[] items = GetItemsSource();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].translation.Equals("") || items[i].vocabularyName.Equals(""))
                {
                    scroller.ScrollTo(i, scrollOffset);
                    return "Cần điền đầy đủ thông tin";
                }
                for (int j = i + 1; j < items.Length; j++)
                {
                    if (items[i].vocabularyName.Equals(items[j].vocabularyName))
                    {
                        scroller.ScrollTo(j, scrollOffset);
                        this.timer = Time.time;
                        return "Từ vừa nhập bị trùng tên";
                    }   
                }
            }
            if (IOController.Folder.vocabularies.Keys.Count > 1)
            {
                IOController.Folder.vocabularies.Clear();
                IOController.Folder.texture.Clear();
            }
            IOController.Folder.vocabularies[folderName] = new Vocabulary[items.Length];
            IOController.CreateDirectory(folderName, "/Image/");
            for (int i = 0; i < items.Length; i++)
            {
                string key = folderName + items[i].vocabularyName;
                IOController.Folder.vocabularies[folderName][i] = items[i].GetVocabulary();
                if (items[i].texture != null)
                {
                    IOController.SaveTexture(items[i].texture, folderName, items[i].vocabularyName);
                    IOController.Folder.texture[key] = items[i].texture;
                } else
                {
                    IOController.Folder.texture[key] = null;
                }
            }
            IOController.SaveData(folderName);
            return null;
        }

        public void UpdateTime()
        {
            this.timer = Time.time;
        }

        public int GetSizeItemSource()
        {
            return ItemsSource.Count;
        }

        public void ResetScroll()
        {
            scroller.ScrollTo(0, scrollOffset);
        }

        public void GoToNextCell()
        {
            if (Time.time - this.timer < 0.2f) return; 
            this.timer = Time.time;
            int currentIndex = Mathf.RoundToInt(currentPosition);
            int nextIndex = currentIndex + 1;
            if (nextIndex >= GetSizeItemSource())
            {
                nextIndex = 0; 
            }
            scroller.ScrollTo(nextIndex, scrollOffset);
        }

        public void BackToLastCell()
        {
            this.timer = Time.time;
            int currentIndex = Mathf.RoundToInt(currentPosition);
            int backIndex = currentIndex - 1;
            if (backIndex < 0)
            {
                backIndex = GetSizeItemSource();
            }
            scroller.ScrollTo(backIndex, scrollOffset);
        }

    }
}
