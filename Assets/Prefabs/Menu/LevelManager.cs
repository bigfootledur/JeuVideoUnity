using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public GameObject menuPrefab;
    public GameObject menu;

    //void Awake()
    //{
    //    DontDestroyOnLoad(this.gameObject);
    //}

    void Update()
    {
        if (SceneManager.GetSceneByName("Menu") != SceneManager.GetActiveScene())
        {
            if (Input.GetButtonDown("Pause"))
            {
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;
                if (!GameObject.Find("Canvas").transform.Find("Menu").gameObject)
                    Instantiate(menuPrefab, GameObject.Find("Canvas").transform);

                menu = GameObject.Find("Canvas").transform.Find("Menu").gameObject;

                if (Time.timeScale != 0)
                    menu.SetActive(false);
                else
                    menu.SetActive(true);
            }
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void SaveCurrentGameState()
    {
        // Some complex things I suppose
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void FactionChosen(int faction)
    {
        PlayerPrefsManager.SetFaction(faction);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
