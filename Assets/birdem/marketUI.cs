using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;

public class MarketUI : MonoBehaviour
{
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI fishText;
    public TextMeshProUGUI messageText;

    public int currency = 10;
    
    private void Start()
    {
        Refresh();
    }

    public void BuyOverdrive()
    {
       // TryBuy("Overdrive", GameManager.Instance.OverdrivePrice); 
        TryBuy("Overdrive", 5);
    }

    public void BuyFrostCharge()
    {
        //TryBuy("FrostCharge", GameManager.Instance.FrostChargePrice);
        TryBuy("Frost Charge", 8);
    } 

    private void TryBuy(string id, int price)
    {
        /*if (GameManager.Instance.GetCurrency() < price)
        {
            if (messageText) messageText.text = "Not enough currency!";
            return;
        }

        SetCurrency(GameManager.Instance.GetCurrency() - price)
        if (messageText) messageText.text = $"Purchased: {id}";
        Refresh();*/

         if (currency < price)
        {
            if (messageText) messageText.text = "Not enough currency!";
            return;
        }

        currency -= price;
        if (messageText) messageText.text = $"Purchased: {id}";
        Refresh();

    }

    private void Refresh()
    {
        //currencyText.text = $"Currency: {GameManager.I.GetTotalCurrency()}";
        currencyText.text = $"Currency";

        /*fishText.text =
            $"Fish:\n" +
            $"Common (1): {GameManager.I.GetFish("Common")}\n" +
            $"Rare (2): {GameManager.I.GetFish("Rare")}\n" +
            $"Legendary (5): {GameManager.I.GetFish("Legendary")}";*/
            fishText.text =
            $"Fish:\n" +
            $"Common (1): \n" +
            $"Rare (2): \n" +
            $"Legendary (5): ";

    }
}
