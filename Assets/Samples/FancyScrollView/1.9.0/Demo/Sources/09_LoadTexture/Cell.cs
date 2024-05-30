/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasingCore;
using TMPro;
using HungwNguyen.MUIP;
using Hungw;
using System.Collections;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;

namespace FancyScrollView.Example09
{
    public class Cell : FancyCell<ItemData>
    {
        readonly EasingFunction alphaEasing = Easing.Get(Ease.OutQuint);

        [SerializeField] TMP_InputField vocabulary, translation;
        [SerializeField] Image background = default;
        [SerializeField] CanvasGroup canvasGroup = default;
        [SerializeField] TMP_Text id, type, subVoca, descriptionVoca, descriptionTrans;
        [SerializeField] RawImage image;
        [SerializeField] private float targetWidth, targetHeight;
        [SerializeField]
        private Texture2D texture;
        [SerializeField] private AutoCompleteScript autoCompleteScript;

        private bool isDelayPlayMusic = false;

        public bool isEdit { get; set; }

        public void SetDescription(string voca, string trans)
        {
            descriptionVoca.text = voca;
            descriptionTrans.text = trans;
        }

        public override void UpdateContent(ItemData itemData)
        {
            type.text = itemData.type;
            subVoca.text = "";
            if (itemData.texture != null)
            {
                image.texture = itemData.texture;
                ResizeImage(itemData.texture);
            }
            else
            {
                image.texture = texture;
                ResizeImage(texture);
            }
            UpdateText(vocabulary, itemData.vocabularyName);
            UpdateText(translation, itemData.translation);
            id.text = $"{this.Index + 1} / {ScrollView.Instance.GetSizeItemSource()}";
            UpdateSibling();
        }

        public void OnEnditVocabulary()
        {
            StartCoroutine(DelayEdit());
        }

        public void OnEnditTranslate()
        {
            if (translation.text == "") return;
            ScrollView.Instance.SetTranslation(Index, translation.text);
        }

        IEnumerator DelayEdit()
        {
            yield return new WaitForEndOfFrame();
            if (APIController.Instance.currentTranslation.ContainsKey(vocabulary.text))
            {
                SetTranslation(APIController.Instance.currentTranslation[vocabulary.text]);
            }
            else
            {
                if (vocabulary.text != "")
                {
                    APIController.Instance.GetTranslatetionFromGoogle(vocabulary.text, this);
                }
                else
                {
                    SetTranslation("");
                }

            }
            if (APIController.Instance.currentType.ContainsKey(vocabulary.text))
            {
                SetType(APIController.Instance.currentType[vocabulary.text]);
            }
            SetVocabulary();
        }

        public void SetTextFromAPI()
        {
            if (isEdit)
                APIController.Instance.GetVocabularyAutoComplete(vocabulary.text, autoCompleteScript, this);
        }

        public void PlayDonwLoadAudioClip()
        {
            if (isDelayPlayMusic)
            {
                return;
            }
            else
            {
                StartCoroutine(DelayPlayMusic());
            }
            APIController.Instance.GetSound(vocabulary.text);
        }

        IEnumerator DelayPlayMusic()
        {
            isDelayPlayMusic = true;
            yield return new WaitForSeconds(1f);
            isDelayPlayMusic = false;
        }

        void UpdateText(TMP_InputField input, string content)
        {
            input.text = content;
            input.GetComponent<CustomInputField>().UpdateState();
        }

        public void SetVocabulary()
        {
            if (vocabulary.text == "") return;
            ScrollView.Instance.SetVocabulary(Index, vocabulary.text);
        }

        public void SetTranslation(string translation)
        {
            if (translation == "") return;
            this.translation.text = translation;
            ScrollView.Instance.SetTranslation(Index, translation);
        }

        public void SetType(string type)
        {
            this.type.text = type;
            ScrollView.Instance.SetType(Index, this.type.text);
        }

        private void SetTexture(Texture2D input)
        {
            ScrollView.Instance.SetTexture(Index, input);
        }

        void UpdateSibling()
        {
            var cells = transform.parent.Cast<Transform>()
                .Select(t => t.GetComponent<Cell>())
                .Where(cell => cell.IsVisible);

            if (Index == cells.Min(x => x.Index))
            {
                transform.SetAsLastSibling();
            }

            if (Index == cells.Max(x => x.Index))
            {
                transform.SetAsFirstSibling();
            }
        }

        public void GoToNextCell()
        {
            Invoke("GoNext", 0.1f);
        }

        private void GoNext()
        {
            ScrollView.Instance.GoToNextCell();
        }

        public override void UpdatePosition(float t)
        {
            const float popAngle = -10;
            const float slideAngle = 20;

            const float popSpan = 0.75f;
            const float slideSpan = 0.25f;

            t = 1f - t;

            var pop = Mathf.Min(popSpan, t) / popSpan;
            var slide = Mathf.Max(0, t - popSpan) / slideSpan;

            transform.localRotation = t < popSpan
                ? Quaternion.Euler(0, 0, popAngle * (1f - pop))
                : Quaternion.Euler(0, 0, slideAngle * slide);

            transform.localPosition = Vector3.left * 500f * slide;

            canvasGroup.alpha = alphaEasing(1f - slide);

            background.color = Color.Lerp(Color.gray, new Color32(246, 237, 118, 255), pop);
        }

        [System.Obsolete]
        public void PickImage(int maxSize)
        {
#if UNITY_ANDROID || UNITY_IOS
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                    if (texture == null)
                    {
                        Hungw.UIManager.Instance.bug.text = "Không thể tải ảnh lên!";
                        return;
                    }
                    SetTexture(texture);
                    ResizeImage(texture);
                }
            });
#else
            var bp = new BrowserProperties();
            bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            bp.filterIndex = 0;

            new FileBrowser().OpenFileBrowser(bp, path =>
            {
                //Load image from local path with UWR
                StartCoroutine(LoadImage(path));
            });
#endif
        }

        [System.Obsolete]
        IEnumerator LoadImage(string path)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Hungw.UIManager.Instance.bug.text = "Không thể tải ảnh lên!";
                }
                else
                {
                    var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                    SetTexture(uwrTexture);
                    ResizeImage(uwrTexture);
                }
            }
        }

        private void ResizeImage(Texture2D texture)
        {
            image.texture = texture;
            float currentWidth = texture.width;
            float currentHeight = texture.height;
            if (currentHeight == currentWidth)
            {
                image.rectTransform.sizeDelta = Vector2.one * 666;
            }
            else if (currentHeight > currentWidth)
            {
                image.rectTransform.sizeDelta = new Vector2(targetHeight / currentHeight * currentWidth, targetHeight);
            }
            else
            {
                image.rectTransform.sizeDelta = new Vector2(targetWidth, targetWidth / currentWidth * currentHeight);
            }
        }

    }
}
