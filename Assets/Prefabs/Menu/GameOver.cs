using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    public GameObject playerExpBar;
    public GameObject gameOverMessage; // Victory or lose
    public GameObject playerLevel;
    public GameObject playerAdditionnalPoints;

    public float timeFadeInEnd;
    public bool timeSet = false;

    public const float EXP_BAR_WIDTH = 670;

    public int currentExperiencePoints = 100;
    public int experiencePointsEarned = 1000;
    public int firstExperiencePoints;
    public int totalExperiencePoints;

    public Vector2 fromPlayerAddtionnalPoints = new Vector2(-245, 70);
    public Vector2 toPlayerAdditionnalPoints = new Vector2(-245, 85);

    public float t;
    public float t2;

    void Awake()
    {
        currentExperiencePoints = PlayerPrefsManager.GetPlayerScore();
        gameOverMessage.GetComponent<Text>().text = ScoreRegister.Winner == GameMaster.PlayerFaction ? "Victory" : "Defeat";
        gameOverMessage.GetComponent<Text>().color = ScoreRegister.Winner == GameMaster.PlayerFaction ? Color.green : Color.red;
        experiencePointsEarned = ScoreRegister.PlayerScore();
        experiencePointsEarned += ScoreRegister.Winner == GameMaster.PlayerFaction ? 100 : 0;

        playerAdditionnalPoints.GetComponent<Text>().text = "+ " + experiencePointsEarned.ToString();
        UpdateExperiencePointsDisplay();
        firstExperiencePoints = currentExperiencePoints;
        totalExperiencePoints = currentExperiencePoints + experiencePointsEarned;

        PlayerPrefsManager.SetPlayerScore(totalExperiencePoints);
        ScoreRegister.ResetScores();
    }

    void Update()
    {
        experiencePointsEarnedFadeIn();
        if(t >= 1)
        {
            if (!timeSet)
            {
                timeSet = true;
                timeFadeInEnd = Time.time;
            }
            if(Time.time - timeFadeInEnd > 0.5f)
                experiencePointsEarnedFadeOut();
        }

        if (t < 1)
        {
            currentExperiencePoints = (int)Mathf.Lerp(firstExperiencePoints, totalExperiencePoints, t);
            t += 0.4f * Time.deltaTime;
        }
        
        UpdateExperiencePointsDisplay();
    }

    public void experiencePointsEarnedFadeOut()
    {
        playerAdditionnalPoints.GetComponent<Text>().color = new Color(playerAdditionnalPoints.GetComponent<Text>().color.r,
                                                               playerAdditionnalPoints.GetComponent<Text>().color.g,
                                                               playerAdditionnalPoints.GetComponent<Text>().color.b,
                                                               Mathf.Lerp(1, 0, t2));
        t2 += 0.6f * Time.deltaTime;
    }

    public void experiencePointsEarnedFadeIn()
    {
        playerAdditionnalPoints.transform.localPosition = Vector3.Lerp(fromPlayerAddtionnalPoints, toPlayerAdditionnalPoints, t * 10);
        playerAdditionnalPoints.GetComponent<Text>().color = new Color(playerAdditionnalPoints.GetComponent<Text>().color.r,
                                                                       playerAdditionnalPoints.GetComponent<Text>().color.g,
                                                                       playerAdditionnalPoints.GetComponent<Text>().color.b,
                                                                       Mathf.Lerp(0, 1, t * 9));
    }

    public void UpdateExperiencePointsDisplay()
    {
        double discriminant;
        double b;
        b = (((2 * ScoreRegister.xpToPassLevel) - ScoreRegister.levelCoef) / (2 * ScoreRegister.levelCoef));
        discriminant = Math.Sqrt(((b * b) + (2 * (currentExperiencePoints / ScoreRegister.levelCoef))));

        double levelAndExp = -b + discriminant;

        playerExpBar.GetComponent<Image>().fillAmount = Math.Abs(Convert.ToSingle(levelAndExp - Math.Truncate(levelAndExp)));
        playerLevel.GetComponent<Text>().text = "Level " + HerosLevel(ScoreRegister.xpToPassLevel, ScoreRegister.levelCoef, currentExperiencePoints);
    }

    /// <summary>
    /// Return the level of the heros relative to his xp points and the rules to pass levels
    /// </summary>
    /// <param name="xpToPassLevel"> How many xp points you need to pass level 1 </param>
    /// <param name="levelCoef"> Each level increase the requires xp points by levelCoef </param>
    /// <param name="experiencePoints"> The current experience points of the heros </param>
    /// <returns></returns>
    public int HerosLevel(float xpToPassLevel, float levelCoef, float experiencePoints)
    {
        double discriminant;
        double b;
        b = (((2 * xpToPassLevel) - levelCoef) / (2 * levelCoef));
        discriminant = Math.Sqrt(((b * b) + (2 * (experiencePoints / levelCoef))));

        double levelAndExp = -b + discriminant;
        return Convert.ToInt32(Math.Truncate(levelAndExp)) + 1;
    }

}
