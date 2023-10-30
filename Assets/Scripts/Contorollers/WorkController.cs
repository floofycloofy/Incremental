using System;
using TMPro;
using UnityEngine;

public class WorkController : MonoBehaviour
{
    /// <summary> ����� ������ �������� ��� (hh:mm:00) </summary>
    private TimeSpan workStartTime = new TimeSpan();
    /// <summary> �����, ����� ��������� �� ������ ������(�� �������� ��� ������������ ������������) </summary>
    private TimeSpan workTravelTime = new TimeSpan();
    /// <summary> ������������ �������� ��� (hh:mm:00) </summary>
    private TimeSpan workingTime = new TimeSpan();

    /// <summary>  ����� �������� �� ������ </summary>
    private DateTime atWorkTime;
    /// <summary>  ����� ���������� �������� ��� </summary>
    private DateTime workFinishTime;
    /// <summary> ����� �������� ����� </summary>
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
    /// <summary>����������� ��������� ������ $/hour</summary>
    private float moneyPerHour;
    /// <summary>�����, ������� ����� ��������� � ��������� ��������</summary>
    private float salary;
    /// <summary>�����, ������� ���������� �� �����</summary>
    private float shiftSalary;

    private int workDayCounter;

    /// <summary>���� ������� �� ����� �� ������</summary>
    [SerializeField] private bool hasWork;
    /// <summary>���� ������. ���� 0 - ����� ������ ���������, ����� �� ���� �� ������</summary>
    [SerializeField] private bool timeToWork;
    /// <summary>���� ������. ���� 1 - ����� ���� �� ������</summary>
    [SerializeField] private bool playerStartWorkProcess;
    /// <summary>���� ������. ���� 2 - ����� ��������</summary>
    [SerializeField] private bool playerWorking;
    /// <summary>���� ������. ���� 3 - ����� ���� �����</summary>
    [SerializeField] private bool playerFinishedWork;
    /// <summary>���� ������. ����� �� ����� ����� �� ������</summary>
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
                //�� �������� �� ������ �� ����� �������� ���, ���������� ������ �� �����
            }
            else if (playerController.IsBusy)
            {
                switch (playerController.TypeOfBusy)
                {
                    default:
                        if (!(progressBarController.CurrentStage == 0))
                        {
                            progressBarController.ChangeBar(0);
                            // ������� �� ���� 0
                        }

                        //������ ���� 0 - ������ ��������, ����� �� ���� �� ������
                        break;
                }
            }
            else
            {
                //������� �� ���� 1 ��� ��� ����� �� �����

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
            //������ ���� 1 - ����� ���� �� ������
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
            //������ ���� 2 - ����� ��������
            progressBarController.StepFill();
        }
        else if ((timeToWork))
        {
            if (time >= atHomeTime)
            {
                shiftSalaryTextBox.text = string.Empty;
                workEnd(time); // ������ ���� 4 - ����� ����
            }
            else
            {
                //������ ���� 3 - ����� ���� �����
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
