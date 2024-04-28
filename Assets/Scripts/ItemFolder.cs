using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hungw
{
    public class ItemFolder : MonoBehaviour
    {
        public void OnClick()
        {
            Debug.Log(this.GetComponentInChildren<TextMeshProUGUI>().text);
        }
    }
}
