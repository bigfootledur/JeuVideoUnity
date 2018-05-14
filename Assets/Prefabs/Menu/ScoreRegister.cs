using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRegister : MonoBehaviour {

    public static float xpToPassLevel = 500;
    public static float levelCoef = 200;

    private static int blueScore;
    private static int redScore;

    private static Faction winner;

    public static void ResetScores()
    {
        blueScore = redScore = 0;
    }

    public static void AddBlueScore(int amount)
    {
        blueScore += amount;
    }

    public static void AddRedScore(int amount)
    {
        redScore += amount;
    }

    public static void RegisterScore()
    {
        if(GameMaster.PlayerFaction.Equals(Faction.Blue))
            PlayerPrefsManager.SetPlayerScore(PlayerPrefsManager.GetPlayerScore() + blueScore);
        else if (GameMaster.PlayerFaction.Equals(Faction.Red))
            PlayerPrefsManager.SetPlayerScore(PlayerPrefsManager.GetPlayerScore() + redScore);
    }

    public static Faction Winner
    {
        get
        {
            return winner;
        }

        set
        {
            winner = value;
        }
    }

    public static int PlayerScore()
    {
        return GameMaster.PlayerFaction.Equals(Faction.Blue) ? blueScore : redScore;
    }
}
