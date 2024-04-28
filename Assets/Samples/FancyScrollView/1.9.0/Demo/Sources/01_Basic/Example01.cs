/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using IO;
using System.Linq;
using UnityEngine;

namespace FancyScrollView.Example01
{
    class Example01 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        public static Example01 Instance { get; private set; }
        void Start()
        {
            UpdateData();
            Instance = this;
        }

        public void UpdateData()
        {
            int cell = IOController.Folder.name.Count;
            var items = Enumerable.Range(1, cell)
                .Select(i => new ItemData($"{i} / {cell}", IOController.Folder.name[i - 1]))
                .ToArray();

            scrollView.UpdateData(items);
        }
    }
}
