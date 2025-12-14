using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Movement / States")]
    public float speed = 0f;
    public bool IsInvincible = false;
    public bool IsUntouchable = false;

    [Header("Economy")]
    [SerializeField] private int currency = 0;

    private int OverdrivePriceCur = 150;
    private int FrostChargePriceCur = 200;
    private int LightningPriceCur = 250;
    private int VortexPriceCur = 250;

    [Header("Refs")]
    public GameObject ship;

    [Header("Upgrade Levels")]
    public int freezeLevel = 0;
    public int overDriveLevel = 1;
    public int lightningLevel = 0;
    public int vortexLevel = 0;

    [Header("Collision / FX")]
    public GameObject collideEffect;
    public GameObject IceCollideEffect;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;

    [Header("UI")]
    [SerializeField] private TMP_Text currencyText;

    // =======================
    // PlayerPrefs Keys
    // =======================
    private const string CURRENCY_KEY   = "CURRENCY";
    private const string OVERDRIVE_KEY  = "OVERDRIVE_LEVEL";
    private const string FREEZE_KEY     = "FREEZE_LEVEL";
    private const string LIGHTNING_KEY  = "LIGHTNING_LEVEL";
    private const string VORTEX_KEY     = "VORTEX_LEVEL";

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

<<<<<<< HEAD
=======
        currency = PlayerPrefs.GetInt(CURRENCY_KEY, currency); 
        SoundManager.Instance?.PlayAmbience();

>>>>>>> sounds
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        LoadAll();          // ✅ her Awake’te load
        UpdateCurrencyUI(); // ✅ UI da güncellensin
    }

    // =======================
    // Save / Load (All)
    // =======================
    public void LoadAll()
    {
        currency       = PlayerPrefs.GetInt(CURRENCY_KEY, 0);
        overDriveLevel = PlayerPrefs.GetInt(OVERDRIVE_KEY, 1);
        freezeLevel    = PlayerPrefs.GetInt(FREEZE_KEY, 0);
        lightningLevel = PlayerPrefs.GetInt(LIGHTNING_KEY, 0);
        vortexLevel    = PlayerPrefs.GetInt(VORTEX_KEY, 0);
    }

    public void SaveAll()
    {
        PlayerPrefs.SetInt(CURRENCY_KEY, currency);
        PlayerPrefs.SetInt(OVERDRIVE_KEY, overDriveLevel);
        PlayerPrefs.SetInt(FREEZE_KEY, freezeLevel);
        PlayerPrefs.SetInt(LIGHTNING_KEY, lightningLevel);
        PlayerPrefs.SetInt(VORTEX_KEY, vortexLevel);
        PlayerPrefs.Save();
    }

    // =======================
    // Currency
    // =======================
    public int GetCurrency() => currency;

    public void SetCurrency(int c)
    {
        currency = Mathf.Max(0, c);
        PlayerPrefs.SetInt(CURRENCY_KEY, currency);
        PlayerPrefs.Save();
        UpdateCurrencyUI();
    }

    public void AddCurrency(int amount)
    {
        currency += Mathf.Max(0, amount);
        PlayerPrefs.SetInt(CURRENCY_KEY, currency);
        PlayerPrefs.Save();
        UpdateCurrencyUI();
    }

    public bool TrySpendCurrency(int amount)
    {
        amount = Mathf.Max(0, amount);
        if (currency < amount) return false;

        currency -= amount;
        PlayerPrefs.SetInt(CURRENCY_KEY, currency);
        PlayerPrefs.Save();
        UpdateCurrencyUI();
        return true;
    }

    // =======================
    // Level getters (senin kodlara uyum)
    // =======================
    public int GetOverDriveLevel() => overDriveLevel;

    // =======================
    // LevelUp functions
    // =======================
    public void LevelUpOverDrive()
    {
        overDriveLevel += 1;
        PlayerPrefs.SetInt(OVERDRIVE_KEY, overDriveLevel);
        PlayerPrefs.Save();
    }

    public void LevelUpFreeze()
    {
        freezeLevel += 1;
        PlayerPrefs.SetInt(FREEZE_KEY, freezeLevel);
        PlayerPrefs.Save();
    }

    public void LevelUpLightning()
    {
        lightningLevel += 1;
        PlayerPrefs.SetInt(LIGHTNING_KEY, lightningLevel);
        PlayerPrefs.Save();
    }

    public void LevelUpVortex()
    {
        vortexLevel += 1;
        PlayerPrefs.SetInt(VORTEX_KEY, vortexLevel);
        PlayerPrefs.Save();
    }

    // =======================
    // Prices
    // =======================
    public int FrostChargePrice() => FrostChargePriceCur;
    public int OverdrivePrice() => OverdrivePriceCur;
    public int LightningPrice() => LightningPriceCur;
    public int VortexPrice() => VortexPriceCur;

    // =======================
    // Speed
    // =======================
    public float GetSpeed() => speed;

    public void SetSpeed(float s)
    {
        speed = s;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.uKey.wasPressedThisFrame)
        {
            ResetAllProgress();
        }
    }

    // =======================
    // UI
    // =======================
    void UpdateCurrencyUI()
    {
        if (currencyText == null)
        {
            currencyText = GameObject.Find("CurrencyText")?.GetComponent<TMP_Text>();
            if (currencyText == null) return;
        }

        currencyText.text = currency.ToString();
    }

    public void ResetAllProgress()
    {
        // Runtime değerleri sıfırla
        currency = 0;

        freezeLevel = 0;
        overDriveLevel = 1;   // başlangıç level’ını korumak istiyorsan 1
        lightningLevel = 0;
        vortexLevel = 0;

        // PlayerPrefs temizle (sadece bizim key’ler)
        PlayerPrefs.DeleteKey("CURRENCY");
        PlayerPrefs.DeleteKey("OVERDRIVE_LEVEL");
        PlayerPrefs.DeleteKey("FREEZE_LEVEL");
        PlayerPrefs.DeleteKey("LIGHTNING_LEVEL");
        PlayerPrefs.DeleteKey("VORTEX_LEVEL");
        PlayerPrefs.Save();

        // UI güncelle
        UpdateCurrencyUI();

        Debug.Log("ALL PROGRESS RESET");
    }

}
