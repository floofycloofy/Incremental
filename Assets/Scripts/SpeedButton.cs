using UnityEngine;

public class SpeedButton : MonoBehaviour
{
    [SerializeField] private int buttonSpeed;

    [SerializeField] private UnityEngine.UI.Button cButton;
    [SerializeField] private UnityEngine.UI.Image img;

    [SerializeField] private Color activeColor;
    [SerializeField] private Color normalColor;

    private void Awake()
    {
        GlobalEventManager.SetSpeed.AddListener(ButtonState);
        cButton.onClick.AddListener(Click);
    }
    private void Click()
    {
        GlobalEventManager.SendSetSpeed(buttonSpeed);
    }

    public void ButtonState(int state)
    {
        if (state == buttonSpeed)
        {
            img.color = activeColor;
        }
        else
        {
            img.color = normalColor;
        }
    }
}
