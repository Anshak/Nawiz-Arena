using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataTranslator : MonoBehaviour
{

    //Original Symbol: [KILLS]0/[DEATHS]0

    public static int DataToKills(string data)
    {

        return int.Parse(data.Substring(7, 1));
    }

    public static int DataToDeaths(string data)
    {
        return int.Parse(data.Substring(17, 1));
    }

    public static string ValuesToData (int kills, int deaths)
    {
        return "[KILLS]" + kills + "/" + "[DEATHS]" + deaths;
    }
}
