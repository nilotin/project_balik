using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class powUpsOnHand : MonoBehaviour
{


    [Header("Inventory")]
    public int maxSlots = 3;
    private List<string> inventory = new List<string>();

    [Header("UI")]
    public List<Image> slotImages;

    [Header("Icons")]
    public Sprite iceIcon;
    public Sprite vortexIcon;
    public Sprite lightningIcon;

    // 🔹 Ekle
    public bool AddPowerUp(string powerUpName)
    {
        if (inventory.Count >= maxSlots)
        {
            return false;
        }

        inventory.Add(powerUpName.ToLower());
        UpdateUI();
        return true;
    }

    // 🔹 Kullan (ilk eleman)
    public void UseFirstPowerUp()
    {
        if (inventory.Count == 0)
        {
            return;
        }

        string powerUp = inventory[0];

        ApplyPowerUp(powerUp);

        inventory.RemoveAt(0);
        UpdateUI();
    }

    // 🔹 PowerUp etkisi
    void ApplyPowerUp(string powerUp)
    {
        switch (powerUp)
        {
            case "ice":
                Debug.Log("ICE power-up kullanıldı");
                break;

            case "vortex":
                Debug.Log("VORTEX power-up kullanıldı");
                break;

            case "lightning":
                Debug.Log("LIGHTNING power-up kullanıldı");
                break;

            default:
                Debug.LogWarning("Bilinmeyen power-up: " + powerUp);
                break;
        }
    }

    // 🔹 UI güncelle
    void UpdateUI()
    {

        for (int i = 0; i < slotImages.Count; i++)
        {
            if (i < inventory.Count)
            {

                slotImages[i].sprite = GetIcon(inventory[i]);
                slotImages[i].enabled = true;
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].enabled = false;
            }
        }
    }

    // 🔹 string → icon
    Sprite GetIcon(string powerUp)
    {
        switch (powerUp)
        {
            case "ice":
                return iceIcon;

            case "vortex":
                return vortexIcon;

            case "lightning":
                return lightningIcon;
        }

        return null;
    }


}
