using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FragmentedBarController : MonoBehaviour
{
    [SerializeField] private List<RectTransform> stagesFillers = new List<RectTransform>();
    [SerializeField] private RectTransform barsStartPosition;

    RectTransform activeBar = null;

    private int steps;
    private int currentStage = -1;
    private float progressStep;

    public int CurrentStage => currentStage;

    public void StartBarFilling(DateTime start, DateTime end)
    {
        TimeSpan delta = end - start;
        steps = delta.Minutes + delta.Hours * 60 + 1;
        progressStep = GetComponent<RectTransform>().rect.width / steps;
    }
    public void StepFill()
    {
        activeBar.sizeDelta = new Vector2(activeBar.sizeDelta.x + progressStep, activeBar.sizeDelta.y);
    }
    public void ChangeBar(int stage)
    {
        float x;
        RectTransform previusBar = activeBar;

        if (activeBar != null)
        {
            x = previusBar.anchoredPosition.x + previusBar.sizeDelta.x;
        }
        else
        {
            x = barsStartPosition.anchoredPosition.x;
        }

        activeBar = stagesFillers[stage];

        currentStage = stage;
        activeBar.anchoredPosition = new Vector2(x, barsStartPosition.anchoredPosition.y);
    }
    public void ClearProgress()
    {
        for(int i = 0; i < stagesFillers.Count; i++)
        {
            stagesFillers[i].sizeDelta = new Vector2(0, stagesFillers[i].sizeDelta.y);
            stagesFillers[i].transform.position = new Vector3(barsStartPosition.transform.position.x, barsStartPosition.transform.position.y, barsStartPosition.transform.position.z);
        }
        activeBar = null;
    }
}