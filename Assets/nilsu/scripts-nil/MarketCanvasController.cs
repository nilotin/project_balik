using UnityEngine;
using UnityEngine.InputSystem;

public class MarketCanvasController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject marketCanvas;
    [SerializeField] private ShipMovement shipMovement;
    [SerializeField] private MarketUI marketUI;

    private bool isOpen;
    private bool playerInRange;

    void Awake()
    {
        marketCanvas.SetActive(false);
        isOpen = false;
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleMarket();
        }
    }

    private void ToggleMarket()
    {
        isOpen = !isOpen;
        marketCanvas.SetActive(isOpen);
        if(isOpen)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        if (shipMovement != null)
            shipMovement.enabled = !isOpen;

        if (isOpen && marketUI != null)
            marketUI.SendMessage("Refresh", SendMessageOptions.DontRequireReceiver);
    }

    public void SetPlayerInRange(bool value)
    {
        playerInRange = value;

        // Kedi alanından çıkınca marketi kapat
        if (!playerInRange && isOpen)
        {
            isOpen = false;
            marketCanvas.SetActive(false);

            if (shipMovement != null)
                shipMovement.enabled = true;
        }
    }

    public bool IsOpen() => isOpen;
}