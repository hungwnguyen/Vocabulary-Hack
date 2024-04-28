using UnityEngine;
using TMPro;

public class PromptScript : MonoBehaviour
{
    public TextMeshProUGUI textPrompt; //Component for text
    public string text; //Text
    AutoCompleteScript mainScript; //Main script

    /// <summary>
    /// Initialize prompt object
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="taggedText">Text with tags</param>
    /// <param name="mainScript">Main script</param>
    public void Init(string text, string taggedText, AutoCompleteScript mainScript) {
        this.text = text;
        textPrompt.text = taggedText;
        this.mainScript = mainScript;
    }

    /// <summary>
    /// User pressed on the prompt
    /// </summary>
    public void PressSelf() {
        mainScript.SelectPrompt(text);
    }
}
