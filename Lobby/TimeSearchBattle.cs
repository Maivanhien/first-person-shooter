using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSearchBattle : MonoBehaviour
{
    [SerializeField] Text timeText;
    private float minute, second;

    private void OnEnable()
    {
        minute = 0f;
        second = 0f;
        timeText.text = "0:00";
    }

    void Update()
    {
        second += Time.deltaTime;
        if(second >= 60f)
        {
            second -= 60f;
            minute += 1f;
        }
        if(second < 10f)
        {
            timeText.text = ((int)minute).ToString() + ":0" + ((int)second).ToString();
        }
        else
        {
            timeText.text = ((int)minute).ToString() + ":" + ((int)second).ToString();
        }
    }
}
