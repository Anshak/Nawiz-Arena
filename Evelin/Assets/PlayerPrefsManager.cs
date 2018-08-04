using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    const string PLAYER_LOGIN_KEY = "player_login";
    const string PLAYER_PASSWORD_KEY = "player_password";

    public static void SetPlayerLogin (string loginName)
    {
        PlayerPrefs.SetString(PLAYER_LOGIN_KEY, loginName);
    }

    public static string GetPlayerLogin()
    {
       return PlayerPrefs.GetString(PLAYER_LOGIN_KEY);
    }

    public static void SetPlayerPassword (string password)
    {
        PlayerPrefs.SetString(PLAYER_PASSWORD_KEY, password);
    }

    public static string GetPlayerPassword()
    {
        return PlayerPrefs.GetString(PLAYER_PASSWORD_KEY);
    }
}
