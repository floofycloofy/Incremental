using System;
using TMPro;
using UnityEngine;

public class WorkController : MonoBehaviour
{
    /// <summary> Время начала рабочего дня (hh:mm:00) </summary>
    private TimeSpan workStartTime = new TimeSpan();
    /// <summary> Время, чтобы добраться до работы пешком(не учитывая все модификаторы передвижения) </summary>
    private TimeSpan workTravelTime = new TimeSpan();
    /// <summary> Длительность рабочего дня (hh:mm:00) </summary>
    private TimeSpan workingTime = new TimeSpan();

    /// <summary>  Время прибытия на работу </summary>
    private DateTime atWorkTime;
    /// <summary>  Время завершения рабочего дня </summary>
    private DateTime workFinishTime;
    /// <summary> Время прибытия домой </summary>
    private DateTime atHomeTime;

    private string workStartText = "START ";
    private string workTravelText = "TRAVEL ";
    private string workEndText = "END ";
    private string moneyPerHourText = "wage per hour ";
    private string salaryText = "salary ";
    private SalaryType salaryType;
    private ScheduleType workSchedule;
    public enum SalaryType
    {
        Day, Week, Month
    }
    public enum ScheduleType
    {
        twoByTwo, fiveByTwo
    }
    /// <summary>Модификатор заработка работы $/hour</summary>
    private float moneyPerHour;
    /// <summary>Сумма, которая будет выплачена в следующую зарплату</summary>
    private float salary;
    /// <summary>Сумма, которая заработана за смену</summary>
    private float shiftSalary;

    private int workDayCounter;

    /// <summary>Флаг устроен ли игрок на работу</summary>
    [SerializeField] private bool hasWork;
    /// <summary>Флаг работы. Этап 0 - время работы наступило, игрок не едет на работу</summary>
    [SerializeField] private bool timeToWork;
    /// <summary>Флаг работы. Этап 1 - игрок едет на работу</summary>
    [SerializeField] private bool playerStartWorkProcess;
    /// <summary>Флаг работы. Этап 2 - игрок работает</summary>
    [SerializeField] private bool playerWorking;
    /// <summary>Флаг работы. Этап 3 - игрок едет домой</summary>
    [SerializeField] private bool playerFinishedWork;
    /// <summary>Флаг работы. Игрок не успел выйти на работу</summary>
    [SerializeField] private bool playerIsLateForWork;

    [SerializeField] FragmentedBarController progressBarController;
    [SerializeField] PlayerController playerController;
    [SerializeField] MoneyController moneyController;

    [SerializeField] private TMP_Text workStartTextBox;
    [SerializeField] private TMP_Text workTravelTextBox;
    [SerializeField] private TMP_Text workEndTextBox;
    [SerializeField] private TMP_Text moneyPerHourTextBox;
    [SerializeField] private TMP_Text salaryTextBox;
    [SerializeField] private TMP_Text shiftSalaryTextBox;

    public void Start()
    {
        UpdatePanel();
        SetWork(new TimeSpan(12, 30, 0), new TimeSpan(1, 0, 0), new TimeSpan(0, 15, 0), 5, SalaryType.Week, ScheduleType.twoByTwo);
        shiftSalaryTextBox.text = string.Empty;
    }

    public void Working(DateTime time)
    {
        if (!timeToWork)
        {
            if (time.Date + workStartTime - playerController.AddPlayerTravelModifier(workTravelTime) ==
               new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0))
            {
                if ((workDayCounter % 4 == 1) || (workDayCounter % 4 == 2))
                {
                    timeToWork = true;

                    workFinishTime = time.Date + workStartTime + workingTime;
                    GlobalEventManager.Time15mPassed.RemoveListener(Working);
                    GlobalEventManager.TimePassed.AddListener(Working);
                    progressBarController.StartBarFilling(time, time.Date + workStartTime + workingTime + playerController.AddPlayerTravelModifier(workTravelTime));
                }
                else
                {
                    paySalary(time);
                }
                workDayCounter += 1;
            }
        }
        if ((!playerStartWorkProcess) && (timeToWork))
        {
            atWorkTime = time + playerController.AddPlayerTravelModifier(workTravelTime);
            if ((playerIsLateForWork == true) || (atWorkTime > workFinishTime))
            {
                playerIsLateForWork = true;
                if (time >= workFinishTime)
                {
                    workEnd(time);
                }
                //Не успевает на работу до конца рабочего дня, пропускает работу до конца
            }
            else if (playerController.IsBusy)
            {
                switch (playerController.TypeOfBusy)
                {
                    default:
                        if (!(progressBarController.CurrentStage == 0))
                        {
                            progressBarController.ChangeBar(0);
                            // Переход на Этап 0
                        }

                        //Работа Этап 0 - Работа началась, Игрок не едет на работу
                        break;
                }
            }
            else
            {
                //Переход на этап 1 так как игрок не занят

                progressBarController.ChangeBar(1);
                playerStartWorkProcess = true;
            }
            progressBarController.StepFill();
        }
        else if (!playerWorking && (timeToWork))
        {
            if (time == atWorkTime)
            {
                progressBarController.ChangeBar(2);
                playerWorking = true;
            }
            //Работа Этап 1 - Игрок едет на работу
            progressBarController.StepFill();
        }
        else if (!playerFinishedWork && (timeToWork))
        {
            if (time >= workFinishTime)
            {
                shiftSalary += moneyPerHour / 60;
                salary += moneyPerHour / 60;

                shiftSalaryTextBox.text = "+ " + shiftSalary.ToString("0.00") + " $";
                salaryTextBox.text = salaryText + salary.ToString("0.00") + " $";

                shiftSalary = 0;

                UpdatePanel();
                progressBarController.ChangeBar(3);
                playerFinishedWork = true;
                atHomeTime = workFinishTime + playerController.AddPlayerTravelModifier(workTravelTime);
            }
            else
            {
                shiftSalary += moneyPerHour / 60;
                salary += moneyPerHour / 60;

                shiftSalaryTextBox.text = "+ " + shiftSalary.ToString("0.00") + " $";
                salaryTextBox.text = salaryText + salary.ToString("0.00") + " $";

            }
            //Работа Этап 2 - Игрок работает
            progressBarController.StepFill();
        }
        else if ((timeToWork))
        {
            if (time >= atHomeTime)
            {
                shiftSalaryTextBox.text = string.Empty;
                workEnd(time); // Работа Этап 4 - Игрок дома
            }
            else
            {
                //Работа Этап 3 - Игрок едет домой
                progressBarController.StepFill();
            }
        }
    }


    public void UpdatePanel()
    {
        if (hasWork)
        {
            workStartTextBox.text = workStartText + workStartTime.ToString("hh':'mm");
            workTravelTextBox.text = workTravelText + playerController.AddPlayerTravelModifier(workTravelTime).ToString("hh':'mm");
            workEndTextBox.text = workEndText + (workStartTime + workingTime).ToString("hh':'mm");
            moneyPerHourTextBox.text = moneyPerHourText + moneyPerHour.ToString("0.00") + " $";
        }
        else
        {
            workStartTextBox.text = workStartText + "--:--";
            workTravelTextBox.text = workStartText + "--:--";
            workEndTextBox.text = workEndText + "--:--";
        }
    }

    public void SetWork(TimeSpan startTime, TimeSpan duration, TimeSpan travelTime,
        float _moneyPerHour, SalaryType typeOfSalary, ScheduleType typeOfSchedule)
    {
        workStartTime = startTime;
        workingTime = duration;
        workTravelTime = travelTime;
        moneyPerHour = _moneyPerHour;
        salaryType = typeOfSalary;
        workSchedule = typeOfSchedule;

        if (workSchedule == ScheduleType.twoByTwo)
        {
            workDayCounter = UnityEngine.Random.Range(1, 4);
        }

        hasWork = true;
        timeToWork = false;
        playerStartWorkProcess = false;
        playerWorking = false;
        playerFinishedWork = false;

        UpdatePanel();
        GlobalEventManager.Time15mPassed.AddListener(Working);
    }

    private void workEnd(DateTime time)
    {
        progressBarController.ClearProgress();

        timeToWork = false;
        playerStartWorkProcess = false;
        playerWorking = false;
        playerFinishedWork = false;
        playerIsLateForWork = false;

        paySalary(time);

        GlobalEventManager.TimePassed.RemoveListener(Working);
        GlobalEventManager.Time15mPassed.AddListener(Working);
    }

    private void paySalary(DateTime time)
    {
        switch (salaryType)
        {
            case SalaryType.Day:
                moneyController.MoneyChange(salary);
                salary = 0;
                break;
            case SalaryType.Week:
                if ((int)time.DayOfWeek == 5)
                {
                    moneyController.MoneyChange(salary);
                    salary = 0;
                }
                break;
            case SalaryType.Month:
                if (((int)time.Day == 10) || ((int)time.Day == 25))
                {
                    moneyController.MoneyChange(salary);
                    salary = 0;
                }
                break;
        }
        salaryTextBox.text = salaryText + salary.ToString("0.00") + " $";
    }


    private int checkStage(DateTime time)
    {




        return 0;
    }

}
