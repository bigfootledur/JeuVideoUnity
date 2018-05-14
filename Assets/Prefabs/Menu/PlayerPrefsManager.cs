using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    public static string FACTION_CHOSEN = "FACTION_CHOSEN";
    public static string PLAYER_SCORE = "PLAYER_SCORE";

    public static void SetFaction(int faction)
    {
        PlayerPrefs.SetInt(FACTION_CHOSEN, faction);
    }
    public static int GetFaction()
    {
        return PlayerPrefs.GetInt(FACTION_CHOSEN);
    }

    public static void SetPlayerScore(int score)
    {
        PlayerPrefs.SetInt(PLAYER_SCORE, score);
    }
    public static int GetPlayerScore()
    {
        return PlayerPrefs.GetInt(PLAYER_SCORE);
    }

}
