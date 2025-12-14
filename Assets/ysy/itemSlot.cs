using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class itemSlot : MonoBehaviour
{
    [Header("Which ability is this slot? (freeze / overdrive / lightning / vortex)")]
    public string abilityType = "freeze";

    [Header("UI Refs")]
    public Button button;
    public Image iconImage;
    public Image lockerImage;
    public TMP_Text priceText;

    [Header("Icons by Level (index = level)")]
    // Örn: iconsByLevel[0] = locked/level0, [1] = level1, [2] = level2, [3] = level3
    public Sprite[] iconsByLevel;

    [Header("Max Level")]
    public int maxLevel = 3;
    public coinUpdater coinUp;

    void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnClickUpgrade);
        }
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        int level = GetLevel();
        int price = GetPrice();

        if(level == 0)
            lockerImage.enabled = true;
        else 
            lockerImage.enabled = false;

        // Price yazısı
        if (priceText != null)
        {
            if (level >= maxLevel)
            {
                priceText.text = "MAX";
            }
            else
            {
                priceText.text = price.ToString();
            }
        }
        coinUp.text.text = GameManager.Instance.GetCurrency().ToString();

        // Icon
        if (iconImage != null && iconsByLevel != null && iconsByLevel.Length > 0)
        {
            int idx = Mathf.Clamp(level, 0, iconsByLevel.Length - 1);
            iconImage.sprite = iconsByLevel[idx];
            iconImage.enabled = (iconImage.sprite != null);
        }

        // Buton aktif mi?
        if (button != null)
        {
            button.interactable = (level < maxLevel);
        }
    }

    void OnClickUpgrade()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        int level = GetLevel();
        if (level >= maxLevel)
        {
            Refresh();
            return;
        }

        int price = GetPrice();

        // Para yoksa çık
        if (!GameManager.Instance.TrySpendCurrency(price))
        {
            // İstersen burada "Not enough money" UI popup vs çağırırsın
            Debug.Log("Not enough currency for " + abilityType);
            Refresh();
            return;
        }

        // LevelUp
        DoLevelUp();

        // UI güncelle
        Refresh();
    }

    int GetLevel()
    {
        switch (abilityType.ToLower())
        {
            case "freeze":
                return GameManager.Instance.freezeLevel;

            case "overdrive":
                return GameManager.Instance.overDriveLevel;

            case "lightning":
                return GameManager.Instance.lightningLevel;

            case "vortex":
                return GameManager.Instance.vortexLevel;

            default:
                Debug.LogWarning("Unknown abilityType: " + abilityType);
                return 0;
        }
    }

    int GetPrice()
    {
        switch (abilityType.ToLower())
        {
            case "freeze":
                return GameManager.Instance.FrostChargePrice();

            case "overdrive":
                return GameManager.Instance.OverdrivePrice();

            case "lightning":
                return GameManager.Instance.LightningPrice();

            case "vortex":
                return GameManager.Instance.VortexPrice();

            default:
                return 999999;
        }
    }

    void DoLevelUp()
    {
        switch (abilityType.ToLower())
        {
            case "freeze":
                GameManager.Instance.LevelUpFreeze();
                break;

            case "overdrive":
                GameManager.Instance.LevelUpOverDrive();
                break;

            case "lightning":
                GameManager.Instance.LevelUpLightning();
                break;

            case "vortex":
                GameManager.Instance.LevelUpVortex();
                break;

            default:
                Debug.LogWarning("Unknown abilityType: " + abilityType);
                break;
        }
    }
}
