/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

namespace FancyScrollView.Example01
{
    class ItemData
    {
        public string id { get; set; }
        public string folderName { get; set; }

        public ItemData(string id, string folderName)
        {
            this.id = id;
            this.folderName = folderName;
        }
    }
}
