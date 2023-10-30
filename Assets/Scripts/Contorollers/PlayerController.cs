using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private WorkController workController;
    [SerializeField] private TimeController timeController;

    private bool isBusy;

    private string typeOfBusy;

    private float travelTimeModifier = 1;

    public bool IsBusy => isBusy;

    public string TypeOfBusy => typeOfBusy;

    private void Awake()
    {
        isBusy = false;
    }
    public TimeSpan AddPlayerTravelModifier(TimeSpan pureTravelTime)
    {
        TimeSpan calculatedTravelTime = pureTravelTime * travelTimeModifier;
        calculatedTravelTime = new TimeSpan(calculatedTravelTime.Hours,calculatedTravelTime.Minutes,0);
        return calculatedTravelTime;
    }
}
