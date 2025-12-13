using UnityEngine;

public class fish : MonoBehaviour
{
    public Rarity rarity;
    public Sprite[] fishes;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        if(rarity == Rarity.Common)
            spriteRenderer.sprite = fishes[0];
        else if(rarity == Rarity.Rare)
            spriteRenderer.sprite = fishes[1];
        else
            spriteRenderer.sprite = fishes[2];
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship"))
        {
            int currentCurrency = GameManager.Instance.GetCurrency();
            if(rarity == Rarity.Common)
                GameManager.Instance.SetCurrency(currentCurrency+1);
            else if(rarity == Rarity.Rare)
                GameManager.Instance.SetCurrency(currentCurrency+3);
            else
                GameManager.Instance.SetCurrency(currentCurrency+5);
        }
        Destroy(gameObject);
    }
}

public enum Rarity
{
    Common,
    Rare,
    Legandary
}

