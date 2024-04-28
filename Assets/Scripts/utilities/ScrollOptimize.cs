using TMPro;
using UnityEngine;

namespace Hungw
{
    public class ScrollOptimize : MonoBehaviour
    {
        private RectTransform rectTransform; 
        private float distance, newHeight; 
        private int maxcurrentindex;
        private float top, bottom;
        private int range = 3;
        public delegate void OnValueChange();
        public OnValueChange onValueChange;
        public int size = 100;

        void Start()
        {
            distance = transform.GetChild(0).position.y - transform.GetChild(1).position.y;
            top = transform.GetChild(0).position.y + distance * range;
            bottom = transform.GetChild(0).position.y;
            maxcurrentindex = transform.childCount;
            //size = folderNames.Length;
            rectTransform = this.GetComponent<RectTransform>();
            float itemHeight = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
            newHeight = (rectTransform.sizeDelta.y - itemHeight * maxcurrentindex) / (maxcurrentindex + 1) + itemHeight;
            onValueChange = SetData;
        }

        private void SetData()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject go = transform.GetChild(i).gameObject;
                go.GetComponentInChildren<TextMeshProUGUI>().text = (i + maxcurrentindex - transform.childCount) + "";
                go.name = (i + maxcurrentindex - transform.childCount) + "";
            }
        }

        public void EventValueChange()
        {
            if (this.transform.GetChild(0).position.y > top && maxcurrentindex < size)
            {
                maxcurrentindex++;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + newHeight);
                transform.GetChild(0).position = transform.GetChild(transform.childCount - 1).position - distance * Vector3.up;
                transform.GetChild(0).SetSiblingIndex(transform.childCount - 1);
                onValueChange();
            }
            if (this.transform.GetChild(range - 1).position.y < bottom && maxcurrentindex > transform.childCount)
            {
                maxcurrentindex--;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - newHeight);
                transform.GetChild(transform.childCount - 1).position = transform.GetChild(0).position + distance * Vector3.up;
                transform.GetChild(transform.childCount - 1).SetSiblingIndex(0);
                onValueChange();
            }
        }

    }
}
