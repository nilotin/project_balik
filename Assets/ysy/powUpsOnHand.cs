using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class powUpsOnHand : MonoBehaviour
{
    public int maxSlots = 3;

    [Header("UI")]
    public List<Image> slotImages;

    [Header("Icons")]
    public Sprite iceIcon;
    public Sprite vortexIcon;
    public Sprite lightningIcon;

    private List<string> inventory = new List<string>();

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

    // ✅ Slot index'ten bak (0,1,2)
    public string PeekAt(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            return null;
        }

        return inventory[index];
    }

    // ✅ Slot index'ten tüket (silince kaydırma otomatik)
    public string ConsumeAt(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            return null;
        }

        string p = inventory[index];
        inventory.RemoveAt(index);
        UpdateUI();
        return p;
    }

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
