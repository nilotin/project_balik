using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI fishText;
    public TextMeshProUGUI messageText;

    [Header("Button Sprites")]
    public Sprite buttonPlain;
    public Sprite buttonLevel1;
    public Sprite buttonLevel2;
    public Sprite buttonLevel3;
    public Sprite buttonLocked; // opsiyon

    [Header("Lock Overlays (child lock images)")]
    public GameObject overdriveLock;
    public GameObject frostLock;
    public GameObject vortexLock;
    public GameObject lightningLock;

    [Header("Buttons (Image)")]
    public UnityEngine.UI.Image overdriveBtnImage;
    public UnityEngine.UI.Image frostBtnImage;
    public UnityEngine.UI.Image vortexBtnImage;
    public UnityEngine.UI.Image lightningBtnImage;

    [Header("Button Background Images (turuncu bar olan Image)")]
    public Image overdriveBg;
    public Image frostBg;
    public Image vortexBg;
    public Image lightningBg;


    private Sprite GetSpriteForLevel(int level)
    {
        switch (level)
        {
            case 0: return buttonPlain;
            case 1: return buttonLevel1;
            case 2: return buttonLevel2;
            default: return buttonLevel3; // 3 ve üstü
        }
    }


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
        GameManager.Instance.SetCurrency(100);
        ResetAllPowerUpLevelsForTest(); // <-- TEST İÇİN
        Refresh();
    }

    private void ResetAllPowerUpLevelsForTest()
{
    PlayerPrefs.DeleteKey("PU_LEVEL_Overdrive");
    PlayerPrefs.DeleteKey("PU_LEVEL_FrostCharge");
    PlayerPrefs.DeleteKey("PU_LEVEL_Vortex");
    PlayerPrefs.DeleteKey("PU_LEVEL_Lightning");

    PlayerPrefs.Save();
    Debug.Log("ALL POWER UP LEVELS RESET (TEST MODE)");
}


    public void BuyOverdrive()
    {
        int price = GameManager.Instance.OverdrivePrice();
        TryBuyLevelUp("Overdrive", price);
    }

    public void BuyFrostCharge()
    {
        int price = GameManager.Instance.FrostChargePrice();
        TryBuyLevelUp("FrostCharge", price);
    }

     public void BuyVortex()
    {
        int price = GameManager.Instance.VortexPrice();
        TryBuyLevelUp("Vortex", price);
        
    }

     public void BuyLightning()
    {
        int price = GameManager.Instance.LightningPrice();
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

        UpdateLockOverlay("Overdrive", overdriveLock);
        UpdateLockOverlay("FrostCharge", frostLock);
        UpdateLockOverlay("Vortex", vortexLock);
        UpdateLockOverlay("Lightning", lightningLock);

        UpdateLevelBar("Overdrive", overdriveBg);
        UpdateLevelBar("FrostCharge", frostBg);
        UpdateLevelBar("Vortex", vortexBg);
        UpdateLevelBar("Lightning", lightningBg);


        UpdateButtonVisual("Overdrive", overdriveBtnImage);
        UpdateButtonVisual("FrostCharge", frostBtnImage);
        UpdateButtonVisual("Vortex", vortexBtnImage);
        UpdateButtonVisual("Lightning", lightningBtnImage);

       // Debug.Log($"Overdrive Lv {GetLevel("Overdrive")} | FrostCharge Lv {GetLevel("FrostCharge")} | Vortex Lv {GetLevel("Vortex")} | Lightning Lv {GetLevel("Lightning")}");
        Debug.Log("Overdrive level = " + GetLevel("Overdrive"));


    }

    private void UpdateLevelBar(string id, Image bg)
{
    if (bg == null) return;

    int level = GetLevel(id);

    switch (level)
    {
        case 0: bg.sprite = buttonPlain; break;
        case 1: bg.sprite = buttonLevel1; break;
        case 2: bg.sprite = buttonLevel2; break;
        default: bg.sprite = buttonLevel3; break; // 3+
    }

    bg.preserveAspect = true;
}


private void UpdateLockOverlay(string id, GameObject lockObj)
{
    if (lockObj == null) return;

    int level = GetLevel(id);

    // 0 ise kilit açık (görünsün), 1+ ise kilit kapalı (kaybolsun)
    lockObj.SetActive(level <= 0);
}

   private void UpdateButtonVisual(string id, UnityEngine.UI.Image img)
{
    if (img == null) return;

    int level = GetLevel(id);

    if (level >= MAX_LEVEL)
    {
        img.sprite = buttonLevel3;
        img.preserveAspect = true;
        return;
    }

    int basePrice = GetBasePrice(id);
    int price = GetNextPrice(basePrice, level);

    if (GameManager.Instance.GetCurrency() < price && buttonLocked != null)
        img.sprite = buttonLocked;
    else
        img.sprite = GetSpriteForLevel(level);

    img.preserveAspect = true;
}

private int GetBasePrice(string id)
{
    if (id == "Overdrive") return GameManager.Instance.OverdrivePrice();
    if (id == "FrostCharge") return GameManager.Instance.FrostChargePrice();
    if (id == "Vortex") return 10;
    if (id == "Lightning") return 15;
    return 999;
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
