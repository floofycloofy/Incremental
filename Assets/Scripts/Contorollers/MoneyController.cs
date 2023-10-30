using UnityEngine;

public class MoneyController : MonoBehaviour
{
    [SerializeField] private float startMoney = 0f;
    [SerializeField] protected float money = 0f;

    private void Start()
    {
        MoneyChange(0f);
    }
    public float MoneyChange(float amount)
    {
        float buffer = amount + money;
        if (buffer >= 0)
        {
            money = buffer;
            GlobalEventManager.SendMoneyChange(money);
            return money;
        }
        else
        {
            GlobalEventManager.SendNotEnoughMoney(money);
            return buffer;
        }
    }
}
