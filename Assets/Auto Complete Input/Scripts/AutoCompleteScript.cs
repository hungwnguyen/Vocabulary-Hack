using System;
using System.Collections;
using IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class PotentialPrompts
{
    public string text;
    public string taggedText;
    public float overlapValue;

    public PotentialPrompts(string text, string taggedText, float overlapValue)
    {
        this.text = text;
        this.taggedText = taggedText;
        this.overlapValue = overlapValue;
    }
}

[Serializable]
public class PromptTag
{
    public int ID;
    public string tag;

    public PromptTag(int ID, string tag)
    {
        this.ID = ID;
        this.tag = tag;
    }
}

[Serializable]
public class Prompt
{
    public string text;

    public Prompt(string text)
    {
        this.text = text;
    }
}

public class AutoCompleteScript : MonoBehaviour
{
    [Header("Components and basic settings")]
    [SerializeField] TMP_InputField inputField; //Component for input field

    [SerializeField] GameObject promptTemplate; //Prompt prefab
    [SerializeField] Transform promptContent; //Prompt content

    private List<Prompt> promptList = new List<Prompt>(); //Prompt list

    [Header("Settings")]
    [SerializeField] string openTagSelected; //Open tag for selection
    [SerializeField] string closeTagSelected; //Close tag for selection

    [SerializeField] bool isCaseSensitive; //Is case sensitive
    [SerializeField] bool isSearchByKeywords; //Is search by keywords or by whole input phrase 

    [SerializeField] int maxCount = 5; //Max prompt count
    [SerializeField] PromptScript defaultPromt;
    [SerializeField] TMP_Text subInput;
    [SerializeField] bool openFolder;

    public Action<string> onTextSelected; //Callback when user selects the text

    /// <summary>
    /// Select prompt (Executes from prompt script)
    /// </summary>
    /// <param name="text">Text</param>
    public void SelectPrompt(string text)
    {
        inputField.text = text;
        subInput.text = text;
        for (int i = 0; i < promptContent.childCount; i++)
        {
            promptContent.GetChild(i).gameObject.SetActive(false);
        }
        if (onTextSelected != null)
        {
            onTextSelected(text);
        }
        if (openFolder)
        {
            StartCoroutine(IOController.OpenFolderRecent(text));
        }
    }

    public void UpdatePromtList(List<string> promptList)
    {
        this.promptList.Clear();
        for (int i = 0; i < promptList.Count; i++)
        {
            this.promptList.Add(new Prompt(promptList[i]));
        }
    }
    /// <summary>
    /// On input changed by user
    /// </summary>
    public void OnInputChanged()
    {
        int count = 0;
        if (string.IsNullOrEmpty(inputField.text.Trim()))
        {
            for (int i = count; i < promptContent.childCount; i++)
            {
                promptContent.GetChild(i).gameObject.SetActive(false);
            }
            subInput.text = "";
            return;
        }
        string targetText = isCaseSensitive ? inputField.text.Trim() : inputField.text.Trim().ToLower();
        if (isSearchByKeywords)
        {
            var keywords = targetText.Split(' ');
            var potentialPrompts = new List<PotentialPrompts>();
            for (int i = 0; i < promptList.Count; i++)
            {
                var promptText = isCaseSensitive ? promptList[i].text : promptList[i].text.ToLower();
                bool isCorrect = true;
                float overlapValue = 0;
                var tagsList = new List<PromptTag>();
                for (int j = 0; j < keywords.Length; j++)
                {
                    if (!promptText.Contains(keywords[j]))
                    {
                        isCorrect = false;
                    }
                    else
                    {
                        overlapValue += ((float)keywords[j].Length) / ((float)promptText.Length);
                        tagsList.Add(new PromptTag(promptText.IndexOf(keywords[j]), openTagSelected));
                        tagsList.Add(new PromptTag(promptText.IndexOf(keywords[j]) + keywords[j].Length, closeTagSelected));
                    }
                }
                if (isCorrect)
                {
                    tagsList.Sort(Comparator);
                    string taggedText = promptList[i].text;
                    for (int j = 0; j < tagsList.Count; j++)
                    {
                        taggedText = taggedText.Insert(tagsList[j].ID, tagsList[j].tag);
                    }
                    potentialPrompts.Add(new PotentialPrompts(promptList[i].text, taggedText, overlapValue));
                }
            }
            for (int i = 0; i < Mathf.Min(maxCount, potentialPrompts.Count); i++)
            {
                var newPrompt = promptContent.childCount > count ? promptContent.GetChild(count).gameObject : Instantiate(promptTemplate, promptContent);
                newPrompt.SetActive(true);
                newPrompt.GetComponent<PromptScript>().Init(potentialPrompts[i].text, potentialPrompts[i].taggedText, this);
                count++;
            }
        }
        else
        {
            var potentialPrompts = new List<PotentialPrompts>();
            for (int i = 0; i < promptList.Count; i++)
            {
                var promptText = isCaseSensitive ? promptList[i].text : promptList[i].text.ToLower();
                bool isCorrect = true;
                float overlapValue = 0;
                overlapValue += ((float)targetText.Length) / ((float)promptText.Length);
                if (!promptText.Contains(targetText))
                {
                    isCorrect = false;
                }
                if (isCorrect)
                {
                    string taggedText = promptList[i].text;
                    taggedText = taggedText.Insert(promptText.IndexOf(targetText) + targetText.Length, closeTagSelected);
                    taggedText = taggedText.Insert(promptText.IndexOf(targetText), openTagSelected);
                    potentialPrompts.Add(new PotentialPrompts(promptList[i].text, taggedText, overlapValue));
                }
            }
            for (int i = 0; i < Mathf.Min(maxCount, potentialPrompts.Count); i++)
            {
                var newPrompt = promptContent.childCount > count ? promptContent.GetChild(count).gameObject : Instantiate(promptTemplate, promptContent);
                newPrompt.SetActive(true);
                newPrompt.GetComponent<PromptScript>().Init(potentialPrompts[i].text, potentialPrompts[i].taggedText, this);
                count++;
            }
        }
        for (int i = count; i < promptContent.childCount; i++)
        {
            promptContent.GetChild(i).gameObject.SetActive(false);
        }
        if (promptContent.GetChild(0).gameObject.activeSelf)
        {
            subInput.text = promptContent.GetChild(0).GetComponent<PromptScript>().text;
        }
        else
        {
            subInput.text = "";
        }
    }


    public void OnEndEdit()
    {
        try
        {
            StartCoroutine(DelayEdit());
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    IEnumerator DelayEdit()
    {
        yield return new WaitForEndOfFrame();
        if (defaultPromt.gameObject.activeSelf)
        {
            defaultPromt.PressSelf();
        }
        else if (openFolder)
        {
            StartCoroutine(IOController.OpenFolderRecent(inputField.text));
        }
    }

    int Comparator(PromptTag first, PromptTag second)
    {
        return first.ID > second.ID ? -1 : 1;
    }

    int ComparatorPrompt(PotentialPrompts first, PotentialPrompts second)
    {
        return first.overlapValue > second.overlapValue ? -1 : 1;
    }
}
