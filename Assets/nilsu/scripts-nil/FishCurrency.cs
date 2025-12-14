using TMPro;
using UnityEngine;

public class FishCurrency : MonoBehaviour
{
    public static FishCurrency Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text fishText;

    private int totalFish = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateUI();
    }

    public void AddFish(int amount)
    {
        totalFish += amount;
        UpdateUI();
    }

    public bool SpendFish(int amount)
    {
        if (totalFish < amount)
            return false;

        totalFish -= amount;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        if (fishText != null)
            fishText.text = totalFish.ToString();
    }

    public int GetTotalFish()
    {
        return totalFish;
    }
}