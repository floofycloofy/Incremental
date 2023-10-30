using TMPro;
using UnityEngine;
using System;

public class DateTimeText : MonoBehaviour
{

    [SerializeField] private TMP_Text TMP;
    private void Awake()
    {
        GlobalEventManager.TimePassed.AddListener(UpdateText);
    }

    private void UpdateText(DateTime newTime)
    {
        TMP.text = newTime.ToString("yyyy.MM.dd HH:mm");
    }
}