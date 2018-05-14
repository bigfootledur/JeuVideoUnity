using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ChooseScenesWindow : Editor
{
    [MenuItem("Open Scene/Menu")]
    public static void OpenMenu()
    {
        OpenScene("Menu");
    }
    [MenuItem("Open Scene/Hero")]
    public static void OpenHero()
    {
        OpenScene("Hero");
    }
    [MenuItem("Open Scene/GameOver")]
    public static void OpenGameOver()
    {
        OpenScene("GameOver");
    }

    public static void OpenScene(string name)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
    }
}
