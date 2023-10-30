using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeekManager : MonoBehaviour
{

    private int weekDay;
    [SerializeField] List<TMP_Text> WeekList = new List<TMP_Text>();
    private void Awake()
    {
        GlobalEventManager.TimeDayPassed.AddListener(dayOfWeekHigliter);
    }

    private void dayOfWeekHigliter(DateTime time)
    {
        weekDay = (int)time.DayOfWeek;
        WeekList[weekDay].color = Color.red;
        if (weekDay == 0)
        {
            WeekList[6].color = Color.white;
        }
        else
        {
            WeekList[weekDay - 1].color = Color.white;
        }
    }
}
