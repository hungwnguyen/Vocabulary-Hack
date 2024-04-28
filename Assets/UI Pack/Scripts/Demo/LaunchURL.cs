using UnityEngine;

namespace HungwNguyen.MUIP
{
    public class LaunchURL : MonoBehaviour
    {
        public void GoToURL(string URL)
        {
            Application.OpenURL(URL);
        }
    }
}