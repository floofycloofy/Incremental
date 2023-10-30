using System;
using UnityEngine.Events;

public class GlobalEventManager
{
    //Игра
    public static UnityEvent GameFreeze = new UnityEvent();
    public static void SendGameFreeze()
    {
        GameFreeze.Invoke();
    }

    //Время
    public static UnityEvent<DateTime> TimePassed = new UnityEvent<DateTime>();
    public static UnityEvent<DateTime> Time15mPassed = new UnityEvent<DateTime>();
    public static UnityEvent<DateTime> TimeDayPassed = new UnityEvent<DateTime>();
    public static UnityEvent<int> SetSpeed = new UnityEvent<int>();
    public static UnityEvent<TimeSpan, TimeSpan> SetSkipTime = new UnityEvent<TimeSpan, TimeSpan>();
    public static void SendTimePassed(DateTime time)
    {
        TimePassed.Invoke(time);
    }
    public static void SendTime15mPassed(DateTime time)
    {
        Time15mPassed.Invoke(time);
    }
    public static void SendDayPassed(DateTime time)
    {
        TimeDayPassed.Invoke(time);
    }
    public static void SendSetSpeed(int speed)
    {
        SetSpeed.Invoke(speed);
    }
    public static void SendSetSkipTime(TimeSpan startTime, TimeSpan endTime)
    {
        SetSkipTime.Invoke(startTime, endTime);
    }

    //Деньги
    public static UnityEvent<float> MoneyChange = new UnityEvent<float>();
    public static UnityEvent<float> NotEnoughMoney = new UnityEvent<float>();
    public static void SendMoneyChange(float amount)
    {
        MoneyChange.Invoke(amount);
    }
    public static void SendNotEnoughMoney(float minusAmount)
    {
        NotEnoughMoney.Invoke(minusAmount);
    }

    //События
    public static UnityEvent<string> TimeToWork = new UnityEvent<string>();

    public static void SendTimeToWork(string type)
    {
        TimeToWork.Invoke(type);
    }
}