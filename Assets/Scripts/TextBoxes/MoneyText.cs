using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{

    [SerializeField] private TMP_Text TMP;
    private void Awake()
    {
        GlobalEventManager.MoneyChange.AddListener(UpdateText);
    }

    private void UpdateText(float newMoney)
    {
        string newText = newMoney.ToString("F2");
        TMP.text = newText + " $";
    }
}
