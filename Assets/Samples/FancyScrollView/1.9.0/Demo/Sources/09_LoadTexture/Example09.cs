/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using IO;

namespace FancyScrollView.Example09
{
    public class Example09 : MonoBehaviour
    {
        private ItemData[] itemData;
        private ItemData[] currentItemData;

        [SerializeField] ScrollView scrollView = default;
        [SerializeField] private Slider slider;

        private int size;
        public static Example09 Instance { get; private set; }
        public void Start()
        {
            StartCreate();
            Instance = this;
        }

        public void StartCreate()
        {
            size = (int)slider.value;
            itemData = new ItemData[size];
            for (int i = 0; i < size; i++)
            {
                itemData[i] = new ItemData();
            }
            scrollView.UpdateData(itemData);
            scrollView.ResetScroll();
        }

        public void UpdateScrollViewFromFile(string name)
        {
            size = IOController.Folder.vocabularies[name].Length;
            slider.value = size;
            itemData = new ItemData[size];
            for (int i = 0; i < size; i++)
            {
                string key = name + IOController.Folder.vocabularies[name][i].vocabularyName;
                if (!IOController.Folder.texture.ContainsKey(key))
                {
                    IOController.Folder.texture[key] = null;
                }
                itemData[i] = new ItemData(
                    IOController.Folder.vocabularies[name][i].vocabularyName,
                    IOController.Folder.vocabularies[name][i].translation,
                    IOController.Folder.vocabularies[name][i].type,
                    IOController.Folder.texture[key]
                    );
            }
            scrollView.UpdateData(itemData);
        }

        public void OnChangeSize()
        {
            size = (int)slider.value;
            currentItemData = new ItemData[(int)slider.value];
            itemData = scrollView.GetItemsSource();
            Array.Copy(itemData, currentItemData, size > itemData.Length ? itemData.Length : size);
            if (size > itemData.Length)
            {
                for (int i = itemData.Length; i < size; i++)
                {
                    currentItemData[i] = new ItemData();
                }
            }
            scrollView.UpdateData(currentItemData);
        }
    }
}
