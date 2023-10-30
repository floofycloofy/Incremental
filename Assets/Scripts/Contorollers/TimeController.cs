using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private DateTime gameTime = new DateTime();
    private DateTime gameStartTime = new DateTime(2021, 1, 1, 12, 0, 0);
    private DateTime realTime = new DateTime();

    private DateTime timeStamp = new DateTime();
    private DateTime gameTimeStamp = new DateTime();

    private bool isSkipping = false;

    public float gameSecondsPerRealSecond = 60.0f;

    private int speedModifier;

    private void Awake()
    {
        timeStamp = realTime.AddSeconds(1);
        gameTime = gameStartTime;
        GlobalEventManager.SetSpeed.AddListener(SetSpeed);
    }

    private void Start()
    {
        GlobalEventManager.SendSetSpeed(1);
    }

    private void Update()
    {

        realTime = realTime.AddSeconds(Time.deltaTime);

        if (realTime.Second != timeStamp.Second)
        {
            for (int i = 0; i < speedModifier; i++)
            {
                gameTime = gameTime.AddSeconds(gameSecondsPerRealSecond);
                GlobalEventManager.SendTimePassed(gameTime);

                if (gameTime.Minute % 15 == 0)
                {
                    GlobalEventManager.SendTime15mPassed(gameTime);
                    if (gameTime.Day != gameTimeStamp.Day)
                    {
                        GlobalEventManager.SendDayPassed(gameTime);
                    }
                }
                gameTimeStamp = gameTime;
            }
            timeStamp = realTime;
        }
        if (isSkipping)
        {

        }
    }

    public void SetSpeed(int newSpeedModifier)
    {
        speedModifier = newSpeedModifier;
    }
}
