using TMPro;
using UnityEngine;

public class MarketUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI fishText;
    public TextMeshProUGUI messageText;

    private void Start()
    {
        GameManager.Instance.SetCurrency(20);
        Refresh();
    }

    public void BuyOverdrive()
    {
        int price = GameManager.Instance.OverdrivePrice();
        TryBuy("Overdrive", price);
    }

    public void BuyFrostCharge()
    {
        int price = GameManager.Instance.FrostChargePrice();
        TryBuy("Frost Charge", price);
    }

    private void TryBuy(string itemName, int price)
    {
        int cur = GameManager.Instance.GetCurrency();

        if (cur < price)
        {
            if (messageText != null) messageText.text = "Not enough currency!";
            Refresh();
            return;
        }

        GameManager.Instance.SetCurrency(cur - price);

        if (messageText != null) messageText.text = $"Purchased: {itemName}";
        Refresh();
    }

    public void SellFish()
    {
        // ŞİMDİLİK BASİT: her sell +5 currency versin
        int current = GameManager.Instance.GetCurrency();
        GameManager.Instance.SetCurrency(current + 5);

        if (messageText != null)
            messageText.text = "Sold fish (+5 currency)";

        Refresh();
    }


    private void Refresh()
    {
        if (currencyText != null)
            currencyText.text = GameManager.Instance.GetCurrency().ToString();

        // Fish sistemin GameManager'da yok; şimdilik bilgi amaçlı gösteriyoruz
        if (fishText != null)
        {
            fishText.text =
                "Fish values:\n" +
                "Common = 1\n" +
                "Rare = 2\n" +
                "Legendary = 5";
        }
    }
}
