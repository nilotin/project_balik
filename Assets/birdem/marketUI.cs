using TMPro;
using UnityEngine;

public class MarketUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI fishText;
    public TextMeshProUGUI messageText;

    private int MAX_LEVEL = 3;

    [Header("Price Scaling")]
    [Range(1.1f, 3f)] public float priceMultiplier = 1.6f;

    private string LevelKey(string id)
    {
        return $"PU_LEVEL_{id}";
    }

    private int GetLevel(string id)
    {
        return PlayerPrefs.GetInt(LevelKey(id), 0);
    }

    private void SetLevel(string id, int level)
    {
        level = Mathf.Clamp(level, 0, MAX_LEVEL);
        PlayerPrefs.SetInt(LevelKey(id), level);
        PlayerPrefs.Save();
    }


    private int GetNextPrice(int basePrice, int currentLevel)
    {
        // currentLevel 0 -> Lv1 fiyatı, 1 -> Lv2 fiyatı, 2 -> Lv3 fiyatı
        float scaled = basePrice * Mathf.Pow(priceMultiplier, currentLevel);
        return Mathf.CeilToInt(scaled);
    }

    private void Start()
    {
        GameManager.Instance.SetCurrency(20);
        Refresh();
    }

    public void BuyOverdrive()
    {
        int price = GameManager.Instance.OverdrivePrice();
        TryBuyLevelUp("Overdrive", price);
    }

    public void BuyFrostCharge()
    {
        int price = GameManager.Instance.FrostChargePrice();
        TryBuyLevelUp("Frost Charge", price);
    }

     public void BuyVortex()
    {
        //int price = GameManager.Instance.FrostChargePrice();
        int price = 10;
        TryBuyLevelUp("Vortex", price);
        
    }

     public void BuyLightning()
    {
        //int price = GameManager.Instance.FrostChargePrice();
        int price = 15;
        TryBuyLevelUp("Lightning", price);
        
    }

private void TryBuyLevelUp(string id, int basePrice)
{
    int level = GetLevel(id);

    if (level >= MAX_LEVEL)
    {
        if (messageText) messageText.text = $"{id} MAX (Lv {MAX_LEVEL})";
        return;
    }

    int price = GetNextPrice(basePrice, level);

    int cur = GameManager.Instance.GetCurrency();
    if (cur < price)
    {
        if (messageText) messageText.text = $"Need {price} (You have {cur})";
        return;
    }

    GameManager.Instance.SetCurrency(cur - price);

    int newLevel = level + 1;
    SetLevel(id, newLevel);

    if (messageText) messageText.text = $"Purchased {id} -> Lv {newLevel} (-{price})";
    Refresh();
}


    private int GetPowerUpLevel(string id)
    {
        // default 0: hiç alınmamış
        return PlayerPrefs.GetInt(LevelKey(id), 0);
    }

    private void SetPowerUpLevel(string id, int level)
    {
        level = Mathf.Clamp(level, 0, MAX_LEVEL);
        PlayerPrefs.SetInt(LevelKey(id), level);
        PlayerPrefs.Save(); // jam için güvenli
    }

    private void Refresh()
    {
        if (currencyText)
            currencyText.text = $": {GameManager.Instance.GetCurrency()}";


        Debug.Log($"Overdrive Lv {GetLevel("Overdrive")} | FrostCharge Lv {GetLevel("FrostCharge")} | Vortex Lv {GetLevel("Vortex")} | Lightning Lv {GetLevel("Lightning")}");

    }

    private void UpdatePriceText(string id, int basePrice, TextMeshProUGUI txt)
{
    if (txt == null) return;

    int level = GetLevel(id);

    if (level >= MAX_LEVEL)
    {
        txt.text = "MAX LEVEL";
        return;
    }

    int price = GetNextPrice(basePrice, level);
    txt.text = $"Lv {level}/{MAX_LEVEL}\nCost: {price}";
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


}
