/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections;
using IO;
using TMPro;
using UnityEngine;

namespace FancyScrollView.Example01
{
    class Cell : FancyCell<ItemData>
    {
        [SerializeField] Animator animator = default;
        [SerializeField] TMP_Text id, folderName;

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }

        public override void UpdateContent(ItemData itemData)
        {
            id.text = itemData.id;
            folderName.text = itemData.folderName;
        }

        public override void UpdatePosition(float position)
        {
            currentPosition = position;

            if (animator.isActiveAndEnabled)
            {
                animator.Play(AnimatorHash.Scroll, -1, position);
            }

            animator.speed = 0;
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;

        void OnEnable() => UpdatePosition(currentPosition);

        public void OpenFolder()
        {
            StartCoroutine(IOController.OpenFolderRecent(folderName.text));
        }

        public void OnClickAnimate()
        {
            StartCoroutine(ZoomInOut());
            SoundManager.CreatePlayFXSound();
        }

        private IEnumerator ZoomInOut()
        {
            float time = 0;
            float animationTime = 0.1f;
            while (true)
            {
                time += Time.deltaTime;
                if (time < animationTime)
                {
                    this.transform.localScale += Vector3.one * Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                else if (time < animationTime * 2.5)
                {
                    this.transform.localScale -= Vector3.one * Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                else
                {
                    break;
                }
            }
            this.transform.localScale = Vector3.one;
        }
    }
}
