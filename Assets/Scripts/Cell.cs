using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hungw
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvas;

        // Update is called once per frame
        void Update()
        {
            if (canvas.alpha == 1)
            {
                canvas.blocksRaycasts = true;
            }
            else
            {
                canvas.blocksRaycasts = false;
            }
        }
    }
}
