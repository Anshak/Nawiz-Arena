using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplatterFade : MonoBehaviour {

    [SerializeField] private float fadeOutTime = 10f;
    private Color tempColor;


    public void TakingDamageBloodSplatter(int _amount)
    {
        Debug.Log("Take damage blood splatter called");
        Image image = GetComponent<Image>();
        tempColor = image.color;
        tempColor.a += (_amount / 100f) + 0.1f;
        image.color = tempColor;
    }

    private void Update()
    {
        Image image = GetComponent<Image>();
        tempColor = image.color;
        //Debug.Log("tempColor.a = " + tempColor.a);

        if (tempColor.a > 0)
        {
            //Debug.Log("Will call the IE");
            StartCoroutine("FadeOut");
        }
    }

    IEnumerator FadeOut() {

        //Debug.Log("FadeOut IE called");
        Image image = GetComponent<Image>();
        tempColor = image.color;

        while (tempColor.a > 0)
        {


            tempColor.a -= Time.deltaTime / fadeOutTime ;
            image.color = tempColor;

            yield return null;
        }

       

        
    }

    internal void Die()
    {
        Image image = GetComponent<Image>();
        tempColor = image.color;
        tempColor.a = 1f;
        image.color = tempColor;
    }
}
