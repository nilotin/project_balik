using UnityEngine;

public class fish : MonoBehaviour
{
    public Rarity rarity;
    public Sprite[] fishes;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        switch (rarity)
        {
            case Rarity.Common:
                spriteRenderer.sprite = fishes[0];
                break;
            case Rarity.Rare:
                spriteRenderer.sprite = fishes[1];
                break;
            case Rarity.Legandary:
                spriteRenderer.sprite = fishes[2];
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ship")) return;

        int value = GetFishValue();
        GameManager.Instance.AddCurrency(value);
        
        SoundManager.Instance?.PlaySFX(SoundManager.Instance.fishPickup);

        Destroy(gameObject);
    }

    int GetFishValue()
    {
        switch (rarity)
        {
            case Rarity.Common: return 1;
            case Rarity.Rare: return 3;
            case Rarity.Legandary: return 5;
            default: return 1;
        }
    }
}

public enum Rarity
{
    Common,
    Rare,
    Legandary
}