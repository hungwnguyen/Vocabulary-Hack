/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using IO;

namespace FancyScrollView.Example09
{
    public class ItemData
    {
        public string vocabularyName;
        public string translation;
        public string type;
        public Texture2D texture;
        public bool isChangeTexture = false;
        public bool isChangeTranslation = false;


        public override string ToString()
        {
            return $"Vocabulary: {vocabularyName}, Translation: {translation}, Type: {type}";
        }

        public ItemData(string vocabularyName, string translation, string type, Texture2D texture)
        {
            this.vocabularyName = vocabularyName;
            this.translation = translation;
            this.type = type;
            this.texture = texture;
        }

        public ItemData(string vocabularyName, string translation, string type)
        {
            this.vocabularyName = vocabularyName;
            this.translation = translation;
            this.type = type;
        }

        public ItemData()
        {
            this.vocabularyName = "";
            this.translation = "";
            this.type = "";
            this.texture = null;
        }

        public Vocabulary GetVocabulary(string texturePath = "")
        {
            return new Vocabulary(this, texturePath);
        }
    }
}
