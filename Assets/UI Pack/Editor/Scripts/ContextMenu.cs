﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.MUIP
{
    public class ContextMenu : Editor
    {
        static string objectPath;

        static void GetObjectPath()
        {
            objectPath = AssetDatabase.GetAssetPath(Resources.Load("MUIP Manager"));
            objectPath = objectPath.Replace("Resources/MUIP Manager.asset", "").Trim();
            objectPath = objectPath + "Prefabs/";
        }

        static void MakeSceneDirty(GameObject source, string sourceName)
        {
            if (Application.isPlaying == false)
            {
                Undo.RegisterCreatedObjectUndo(source, sourceName);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        static void ShowErrorDialog()
        {
            EditorUtility.DisplayDialog("UI Pack", "Cannot create the object due to missing manager file. " +
                    "Make sure you have 'MUIP Manager' file in UI Pack > Resources folder.", "Okay");
        }

        static void UpdateCustomEditorPath()
        {
            string mainPath = AssetDatabase.GetAssetPath(Resources.Load("MUIP Manager"));
            mainPath = mainPath.Replace("Resources/MUIP Manager.asset", "").Trim();
            string darkPath = mainPath + "Skins/MUI Skin Dark.guiskin";
            string lightPath = mainPath + "Skins/MUI Skin Light.guiskin";

            EditorPrefs.SetString("MUIP.CustomEditorDark", darkPath);
            EditorPrefs.SetString("MUIP.CustomEditorLight", lightPath);
        }

        static void CreateObject(string resourcePath)
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + resourcePath + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;

                try
                {
                    if (Selection.activeGameObject == null)
                    {
                        var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                        clone.transform.SetParent(canvas.transform, false);
                    }

                    else { clone.transform.SetParent(Selection.activeGameObject.transform, false); }

                    clone.name = clone.name.Replace("(Clone)", "").Trim();
#if !UNITY_2021_3_OR_NEWER || UNITY_2022_1_OR_NEWER
                    MakeSceneDirty(clone, clone.name);
#endif
                }

                catch
                {
                    CreateCanvas();
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    clone.transform.SetParent(canvas.transform, false);
                    clone.name = clone.name.Replace("(Clone)", "").Trim();
#if !UNITY_2021_3_OR_NEWER || UNITY_2022_1_OR_NEWER
                    MakeSceneDirty(clone, clone.name);
#endif
                }

                Selection.activeObject = clone;
            }

            catch { ShowErrorDialog(); }
        }

        static void CreateButton(string resourcePath)
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + resourcePath + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;

                try
                {
                    if (Selection.activeGameObject == null)
                    {
                        var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                        clone.transform.SetParent(canvas.transform, false);
                    }

                    else { clone.transform.SetParent(Selection.activeGameObject.transform, false); }

                    clone.name = "Button";
                    LayoutRebuilder.ForceRebuildLayoutImmediate(clone.GetComponent<RectTransform>());
#if !UNITY_2021_3_OR_NEWER || UNITY_2022_1_OR_NEWER
                    MakeSceneDirty(clone, clone.name);
#endif
                }

                catch
                {
                    CreateCanvas();
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    clone.transform.SetParent(canvas.transform, false);
                    clone.name = "Button";
#if !UNITY_2021_3_OR_NEWER || UNITY_2022_1_OR_NEWER
                    MakeSceneDirty(clone, clone.name);
#endif
                }

                Selection.activeObject = clone;
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("GameObject/UI Pack/Canvas", false, 8)]
        static void CreateCanvas()
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + "Other/Canvas" + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Selection.activeObject = clone;
                MakeSceneDirty(clone, clone.name);
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("Tools/UI Pack/Show UI Manager %#M")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("MUIP Manager");

            if (Selection.activeObject == null)
                Debug.Log("<b>[UI Pack]</b>Can't find a file named 'MUIP Manager'. Make sure you have 'MUIP Manager' file in Resources folder.");
        }

        [MenuItem("GameObject/UI Pack/Button/Standard", false, 8)]
        static void BDEF()
        {
            CreateButton("Button/Basic - Outline/Standard");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Standard", false, 8)]
        static void BBST()
        {
            CreateButton("Button/Basic/Standard");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/White", false, 8)]
        static void BBWHI()
        {
            CreateButton("Button/Basic/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Gray", false, 8)]
        static void BBGR()
        {
            CreateButton("Button/Basic/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Blue", false, 8)]
        static void BBBL()
        {
            CreateButton("Button/Basic/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Brown", false, 8)]
        static void BBBRW()
        {
            CreateButton("Button/Basic/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Green", false, 8)]
        static void BBGRE()
        {
            CreateButton("Button/Basic/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Night", false, 8)]
        static void BBNI()
        {
            CreateButton("Button/Basic/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Orange", false, 8)]
        static void BBOR()
        {
            CreateButton("Button/Basic/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Pink", false, 8)]
        static void BBPIN()
        {
            CreateButton("Button/Basic/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Purple", false, 8)]
        static void BBPURP()
        {
            CreateButton("Button/Basic/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic/Red", false, 8)]
        static void BBRED()
        {
            CreateButton("Button/Basic/Red");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/White", false, 8)]
        static void BGWHI()
        {
            CreateButton("Button/Basic - Gradient/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Gray", false, 8)]
        static void BGGR()
        {
            CreateButton("Button/Basic - Gradient/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Blue", false, 8)]
        static void BGBL()
        {
            CreateButton("Button/Basic - Gradient/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Brown", false, 8)]
        static void BGBRW()
        {
            CreateButton("Button/Basic - Gradient/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Green", false, 8)]
        static void BGGRE()
        {
            CreateButton("Button/Basic - Gradient/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Night", false, 8)]
        static void BGNI()
        {
            CreateButton("Button/Basic - Gradient/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Orange", false, 8)]
        static void BGOR()
        {
            CreateButton("Button/Basic - Gradient/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Pink", false, 8)]
        static void BGPIN()
        {
            CreateButton("Button/Basic - Gradient/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Purple", false, 8)]
        static void BGPURP()
        {
            CreateButton("Button/Basic - Gradient/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Gradient/Red", false, 8)]
        static void BGRED()
        {
            CreateButton("Button/Basic - Gradient/Red");
        }  

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Standard", false, 8)]
        static void BOWHS()
        {
            CreateButton("Button/Basic - Outline/Standard");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/White", false, 8)]
        static void BOWHI()
        {
            CreateButton("Button/Basic - Outline/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Gray", false, 8)]
        static void BOGR()
        {
            CreateButton("Button/Basic - Outline/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Blue", false, 8)]
        static void BOBL()
        {
            CreateButton("Button/Basic - Outline/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Brown", false, 8)]
        static void BOBRW()
        {
            CreateButton("Button/Basic - Outline/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Green", false, 8)]
        static void BOGRE()
        {
            CreateButton("Button/Basic - Outline/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Night", false, 8)]
        static void BONI()
        {
            CreateButton("Button/Basic - Outline/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Orange", false, 8)]
        static void BOOR()
        {
            CreateButton("Button/Basic - Outline/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Pink", false, 8)]
        static void BOPIN()
        {
            CreateButton("Button/Basic - Outline/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Purple", false, 8)]
        static void BOPURP()
        {
            CreateButton("Button/Basic - Outline/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline/Red", false, 8)]
        static void BORED()
        {
            CreateButton("Button/Basic - Outline/Red");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/White", false, 8)]
        static void BOGWHI()
        {
            CreateButton("Button/Basic - Outline Gradient/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Gray", false, 8)]
        static void BOGBGR()
        {
            CreateButton("Button/Basic - Outline Gradient/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Blue", false, 8)]
        static void BOGBL()
        {
            CreateButton("Button/Basic - Outline Gradient/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Brown", false, 8)]
        static void BOGBRW()
        {
            CreateButton("Button/Basic - Outline Gradient/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Green", false, 8)]
        static void BOGGRE()
        {
            CreateButton("Button/Basic - Outline Gradient/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Night", false, 8)]
        static void BOGNI()
        {
            CreateButton("Button/Basic - Outline Gradient/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Orange", false, 8)]
        static void BOGOR()
        {
            CreateButton("Button/Basic - Outline Gradient/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Pink", false, 8)]
        static void BOGPIN()
        {
            CreateButton("Button/Basic - Outline Gradient/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Purple", false, 8)]
        static void BOGPURP()
        {
            CreateButton("Button/Basic - Outline Gradient/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Basic - Outline Gradient/Red", false, 8)]
        static void BOGRED()
        {
            CreateButton("Button/Basic - Outline Gradient/Red");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Standard", false, 8)]
        static void BRS()
        {
            CreateButton("Button/Rounded/Standard");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/White", false, 8)]
        static void BRW()
        {
            CreateButton("Button/Rounded/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Gray", false, 8)]
        static void BRG()
        {
            CreateButton("Button/Rounded/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Blue", false, 8)]
        static void BRB()
        {
            CreateButton("Button/Rounded/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Brown", false, 8)]
        static void BRBR()
        {
            CreateButton("Button/Rounded/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Green", false, 8)]
        static void BRGR()
        {
            CreateButton("Button/Rounded/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Night", false, 8)]
        static void BRN()
        {
            CreateButton("Button/Rounded/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Orange", false, 8)]
        static void BRO()
        {
            CreateButton("Button/Rounded/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Pink", false, 8)]
        static void BRP()
        {
            CreateButton("Button/Rounded/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Purple", false, 8)]
        static void BRPU()
        {
            CreateButton("Button/Rounded/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded/Red", false, 8)]
        static void BRR()
        {
            CreateButton("Button/Rounded/Red");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/White", false, 8)]
        static void BRGW()
        {
            CreateButton("Button/Rounded - Gradient/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Gray", false, 8)]
        static void BRGG()
        {
            CreateButton("Button/Rounded - Gradient/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Blue", false, 8)]
        static void BRGB()
        {
            CreateButton("Button/Rounded - Gradient/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Brown", false, 8)]
        static void BRGBR()
        {
            CreateButton("Button/Rounded - Gradient/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Green", false, 8)]
        static void BRGGR()
        {
            CreateButton("Button/Rounded - Gradient/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Night", false, 8)]
        static void BRGN()
        {
            CreateButton("Button/Rounded - Gradient/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Orange", false, 8)]
        static void BRGO()
        {
            CreateButton("Button/Rounded - Gradient/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Pink", false, 8)]
        static void BRGP()
        {
            CreateButton("Button/Rounded - Gradient/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Purple", false, 8)]
        static void BRGPU()
        {
            CreateButton("Button/Rounded - Gradient/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Gradient/Red", false, 8)]
        static void BRGRE()
        {
            CreateButton("Button/Rounded - Gradient/Red");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Standard", false, 8)]
        static void BROS()
        {
            CreateButton("Button/Rounded - Outline/Standard");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/White", false, 8)]
        static void BROW()
        {
            CreateButton("Button/Rounded - Outline/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Gray", false, 8)]
        static void BROG()
        {
            CreateButton("Button/Rounded - Outline/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Blue", false, 8)]
        static void BROB()
        {
            CreateButton("Button/Rounded - Outline/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Brown", false, 8)]
        static void BROBR()
        {
            CreateButton("Button/Rounded - Outline/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Green", false, 8)]
        static void BROGR()
        {
            CreateButton("Button/Rounded - Outline/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Night", false, 8)]
        static void BRON()
        {
            CreateButton("Button/Rounded - Outline/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Orange", false, 8)]
        static void BROO()
        {
            CreateButton("Button/Rounded - Outline/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Pink", false, 8)]
        static void BROP()
        {
            CreateButton("Button/Rounded - Outline/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Purple", false, 8)]
        static void BROPU()
        {
            CreateButton("Button/Rounded - Outline/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline/Red", false, 8)]
        static void BRORE()
        {
            CreateButton("Button/Rounded - Outline/Red");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/White", false, 8)]
        static void BROGW()
        {
            CreateButton("Button/Rounded - Outline Gradient/White");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Gray", false, 8)]
        static void BROGG()
        {
            CreateButton("Button/Rounded - Outline Gradient/Gray");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Blue", false, 8)]
        static void BROGB()
        {
            CreateButton("Button/Rounded - Outline Gradient/Blue");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Brown", false, 8)]
        static void BROGBR()
        {
            CreateButton("Button/Rounded - Outline Gradient/Brown");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Green", false, 8)]
        static void BROGGR()
        {
            CreateButton("Button/Rounded - Outline Gradient/Green");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Night", false, 8)]
        static void BROGN()
        {
            CreateButton("Button/Rounded - Outline Gradient/Night");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Orange", false, 8)]
        static void BROGO()
        {
            CreateButton("Button/Rounded - Outline Gradient/Orange");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Pink", false, 8)]
        static void BROGP()
        {
            CreateButton("Button/Rounded - Outline Gradient/Pink");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Purple", false, 8)]
        static void BROGPU()
        {
            CreateButton("Button/Rounded - Outline Gradient/Purple");
        }

        [MenuItem("GameObject/UI Pack/Button/Rounded - Outline Gradient/Red", false, 8)]
        static void BROGRE()
        {
            CreateButton("Button/Rounded - Outline Gradient/Red");
        }

        [MenuItem("GameObject/UI Pack/Charts/Pie Chart", false, 8)]
        static void CPC()
        {
            CreateObject("Charts/Pie Chart");
        }

        [MenuItem("GameObject/UI Pack/Context Menu/Standard", false, 8)]
        static void CTXM()
        {
            CreateObject("Context Menu/Context Menu");
        }

        [MenuItem("GameObject/UI Pack/Dropdown/Standard", false, 8)]
        static void DSD()
        {
            CreateObject("Dropdown/Dropdown");
        }

        [MenuItem("GameObject/UI Pack/Dropdown/Multi Select", false, 8)]
        static void DMSD()
        {
            CreateObject("Dropdown/Dropdown - Multi Select");
        }

        [MenuItem("GameObject/UI Pack/Horizontal Selector/Standard", false, 8)]
        static void HSS()
        {
            CreateObject("Horizontal Selector/Horizontal Selector");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Multi-Line", false, 8)]
        static void IFFML()
        {
            CreateObject("Input Field/Input Field - Multi-Line");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Fading (Left Aligned)", false, 8)]
        static void IFFLA()
        {
            CreateObject("Input Field/Input Field - Fading (Left)");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Fading (Middle Aligned)", false, 8)]
        static void IFFMA()
        {
            CreateObject("Input Field/Input Field - Fading (Middle)");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Fading (Right Aligned)", false, 8)]
        static void IFFRA()
        {
            CreateObject("Input Field/Input Field - Fading (Right)");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Standard (Left Aligned)", false, 8)]
        static void IFSLA()
        {
            CreateObject("Input Field/Input Field - Standard (Left)");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Standard (Middle Aligned)", false, 8)]
        static void IFSMA()
        {
            CreateObject("Input Field/Input Field - Standard (Middle)");
        }

        [MenuItem("GameObject/UI Pack/Input Field/Standard (Right Aligned)", false, 8)]
        static void IFSRA()
        {
            CreateObject("Input Field/Input Field - Standard (Right)");
        }

        [MenuItem("GameObject/UI Pack/List View/Custom", false, 8)]
        static void LVC()
        {
            CreateObject("List View/List View Custom");
        }

        [MenuItem("GameObject/UI Pack/List View/Dynamic (Experimental)", false, 8)]
        static void LVD()
        {
            CreateObject("List View/List View");
        }

        [MenuItem("GameObject/UI Pack/Modal Window/Style 1", false, 8)]
        static void MWSS()
        {
            CreateObject("Modal Window/Style 1");
        }

        [MenuItem("GameObject/UI Pack/Modal Window/Style 2", false, 8)]
        static void MWSSS()
        {
            CreateObject("Modal Window/Style 2");
        }

        [MenuItem("GameObject/UI Pack/Movable Window/Standard", false, 8)]
        static void MVWSSWT()
        {
            CreateObject("Movable Window/Movable Window");
        }

        [MenuItem("GameObject/UI Pack/Notification/Fade Animation", false, 8)]
        static void NSN()
        {
            CreateObject("Notification/Fading Notification");
        }

        [MenuItem("GameObject/UI Pack/Notification/Popup Animation", false, 8)]
        static void NSP()
        {
            CreateObject("Notification/Popup Notification");
        }

        [MenuItem("GameObject/UI Pack/Notification/Slide Animation", false, 8)]
        static void NSS()
        {
            CreateObject("Notification/Sliding Notification");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Standard", false, 8)]
        static void PBS()
        {
            CreateObject("Progress Bar/PB - Standard");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Radial Thin", false, 8)]
        static void PBRT()
        {
            CreateObject("Progress Bar/PB - Radial (Thin)");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Radial Light", false, 8)]
        static void PBRL()
        {
            CreateObject("Progress Bar/PB - Radial (Light)");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Radial Regular", false, 8)]
        static void PBRR()
        {
            CreateObject("Progress Bar/PB - Radial (Regular)");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Radial Bold", false, 8)]
        static void PBRB()
        {
            CreateObject("Progress Bar/PB - Radial (Bold)");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Radial Filled Horizontal", false, 8)]
        static void PBRFH()
        {
            CreateObject("Progress Bar/PB - Radial Filled Horizontal");
        }

        [MenuItem("GameObject/UI Pack/Progress Bar/Radial Filled Vertical", false, 8)]
        static void PBRFV()
        {
            CreateObject("Progress Bar/PB - Radial Filled Vertical");
        }

        [MenuItem("GameObject/UI Pack/Spinner/Standard Fill", false, 8)]
        static void PBLSF()
        {
            CreateObject("Spinner/Spinner - Standard Fill");
        }

        [MenuItem("GameObject/UI Pack/Spinner/Standard Run", false, 8)]
        static void PBLSR()
        {
            CreateObject("Spinner/Spinner - Standard Run");
        }

        [MenuItem("GameObject/UI Pack/Spinner/Radial Material", false, 8)]
        static void PBLRM()
        {
            CreateObject("Spinner/Spinner - Radial Material");
        }

        [MenuItem("GameObject/UI Pack/Spinner/Radial Pie", false, 8)]
        static void PBLRP()
        {
            CreateObject("Spinner/Spinner - Radial Pie");
        }

        [MenuItem("GameObject/UI Pack/Spinner/Radial Run", false, 8)]
        static void PBLRR()
        {
            CreateObject("Spinner/Spinner - Radial Run");
        }

        [MenuItem("GameObject/UI Pack/Spinner/Radial Trapez", false, 8)]
        static void PBLRT()
        {
            CreateObject("Spinner/Spinner - Radial Trapez");
        }

        [MenuItem("GameObject/UI Pack/Scrollbar/Standard", false, 8)]
        static void SCS()
        {
            CreateObject("Scrollbar/Scrollbar");
        }

        [MenuItem("GameObject/UI Pack/Slider/Standard/Standard", false, 8)]
        static void SLS()
        {
            CreateObject("Slider/Standard/Slider - Standard");
        }

        [MenuItem("GameObject/UI Pack/Slider/Standard/Standard (Input)", false, 8)]
        static void SLI()
        {
            CreateObject("Slider/Standard/Slider - Standard (Input)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Standard/Standard (Popup)", false, 8)]
        static void SLSP()
        {
            CreateObject("Slider/Standard/Slider - Standard (Popup)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Standard/Standard (Value)", false, 8)]
        static void SLSV()
        {
            CreateObject("Slider/Standard/Slider - Standard (Value)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Gradient/Gradient", false, 8)]
        static void SLG()
        {
            CreateObject("Slider/Gradient/Slider - Gradient");
        }

        [MenuItem("GameObject/UI Pack/Slider/Gradient/Gradient (Input)", false, 8)]
        static void SLGI()
        {
            CreateObject("Slider/Gradient/Slider - Gradient (Input)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Gradient/Gradient (Popup)", false, 8)]
        static void SLGP()
        {
            CreateObject("Slider/Gradient/Slider - Gradient (Popup)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Gradient/Gradient (Value)", false, 8)]
        static void SLGV()
        {
            CreateObject("Slider/Gradient/Slider - Gradient (Value)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Outline/Outline", false, 8)]
        static void SLO()
        {
            CreateObject("Slider/Outline/Slider - Outline");
        }

        [MenuItem("GameObject/UI Pack/Slider/Outline/Outline (Input)", false, 8)]
        static void SLOI()
        {
            CreateObject("Slider/Outline/Slider - Outline (Input)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Outline/Outline (Popup)", false, 8)]
        static void SLOP()
        {
            CreateObject("Slider/Outline/Slider - Outline (Popup)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Outline/Outline (Value)", false, 8)]
        static void SLOV()
        {
            CreateObject("Slider/Outline/Slider - Outline (Value)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Radial/Radial", false, 8)]
        static void SLR()
        {
            CreateObject("Slider/Radial/Slider - Radial");
        }

        [MenuItem("GameObject/UI Pack/Slider/Radial/Radial (Gradient)", false, 8)]
        static void SLRG()
        {
            CreateObject("Slider/Radial/Slider - Radial (Gradient)");
        }

        [MenuItem("GameObject/UI Pack/Slider/Range/Range", false, 8)]
        static void SLRA()
        {
            CreateObject("Slider/Range/Slider - Range");
        }

        [MenuItem("GameObject/UI Pack/Switch/Standard", false, 8)]
        static void SWS()
        {
            CreateObject("Switch/Switch - Standard");
        }

        [MenuItem("GameObject/UI Pack/Toggle/Standard", false, 8)]
        static void TSTST()
        {
            CreateObject("Toggle/Toggle - Standard");
        }

        [MenuItem("GameObject/UI Pack/Toggle/Standard (Light)", false, 8)]
        static void TSTL()
        {
            CreateObject("Toggle/Toggle - Standard (Light)");
        }

        [MenuItem("GameObject/UI Pack/Toggle/Standard (Regular)", false, 8)]
        static void TSTR()
        {
            CreateObject("Toggle/Toggle - Standard (Regular)");
        }

        [MenuItem("GameObject/UI Pack/Toggle/Standard (Bold)", false, 8)]
        static void TSTB()
        {
            CreateObject("Toggle/Toggle - Standard (Bold)");
        }

        [MenuItem("GameObject/UI Pack/Toggle/Toggle Group Panel", false, 8)]
        static void TTGP()
        {
            CreateObject("Toggle/Toggle Group Panel");
        }

        [MenuItem("GameObject/UI Pack/Tooltip/Tooltip Manager", false, 8)]
        static void TTS()
        {
            CreateObject("Tooltip/Tooltip");
        }

        [MenuItem("GameObject/UI Pack/Window Manager/Standard", false, 8)]
        static void MWM()
        {
            CreateObject("Window Manager/Window Manager");
        }
    }
}
#endif