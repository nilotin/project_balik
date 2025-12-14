using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float speed = 0;
    public bool IsInvincible = false;
    public bool IsUntouchable = false;
    [SerializeField]private int currency = 0;
    private int OverdrivePriceCur = 15;
    private int FrostChargePriceCur = 15;
    public GameObject ship;

    public int freezeLevel = 0;
    public int overDriveLevel = 1;
    public int lightningLevel = 0;
    public int vortexLevel = 0;


    public GameObject collideEffect;
    public GameObject IceCollideEffect;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;



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
    }

    public int FrostChargePrice()
    {
        return FrostChargePriceCur;
    }   
    public int OverdrivePrice()
    {
        return OverdrivePriceCur;
    }  
}
