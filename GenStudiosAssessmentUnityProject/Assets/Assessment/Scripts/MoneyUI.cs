using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] Text moneyText;

    void Start()
    {
        PlayerController.onMoneyChanged += OnMoneyChanged;
    }

    void OnDestroy()
    {
        PlayerController.onMoneyChanged -= OnMoneyChanged;
    }
    
    // just a simple listener for assessment
    void OnMoneyChanged(int newValue)
    {
        moneyText.text = newValue.ToString();
    }
}
