using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float speed = 0;
    public bool IsInvincible = false;
    public bool IsUntouchable = false;
    [SerializeField]private int currency = 0;
    private int OverdrivePriceCur = 15;
    private int FrostChargePriceCur = 20;
    private int LightningPriceCur = 25;
    private int VortexPriceCur = 25;

    public GameObject ship;

    public int freezeLevel = 0;
    public int overDriveLevel = 1;
    public int lightningLevel = 0;
    public int vortexLevel = 0;


    public GameObject collideEffect;
    public GameObject IceCollideEffect;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;

    private const string CURRENCY_KEY = "CURRENCY";



    public static GameManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        currency = PlayerPrefs.GetInt(CURRENCY_KEY, currency); 


        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }



    public float GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(float s)
    {
        speed = s;
    }
    public int GetOverDriveLevel()
    {
        return overDriveLevel;
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void SetCurrency(int c)
    {
        currency = c;
        PlayerPrefs.SetInt(CURRENCY_KEY, currency);
        PlayerPrefs.Save();
    }

    public bool TrySpendCurrency(int amount)
    {
        if (currency < amount) return false;
        SetCurrency(currency - amount);
        return true;
    }

    public int FrostChargePrice()
    {
        return FrostChargePriceCur;
    }   
    public int OverdrivePrice()
    {
        return OverdrivePriceCur;
    }  

    public int LightningPrice()
    {
        return LightningPriceCur;   
    }

    public int VortexPrice()
    {
        return VortexPriceCur;
    }
    
    public void AddCurrency(int amount)
    {
        currency += amount;
        UpdateCurrencyUI();
    }

    [SerializeField] private TMP_Text currencyText;

    void UpdateCurrencyUI()
    {
        currencyText.text = currency.ToString();
    }

}
