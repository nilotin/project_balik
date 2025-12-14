using UnityEngine;
using UnityEngine.UI; // UI bileşenlerini kullanmak için

public class HealthUI : MonoBehaviour
{
    [Tooltip("Sırasıyla Heart1, Heart2, Heart3 UI Image bileşenlerini buraya sürükleyin.")]
    public Image[] hearts; 

    void OnEnable()
    {
        // ShipHealth script'inden gelen olayları dinlemeye başla
        ShipHealth.OnHealthChanged += UpdateHealthDisplay;
    }

    void OnDisable()
    {
        // Script devre dışı bırakıldığında dinlemeyi bırak
        ShipHealth.OnHealthChanged -= UpdateHealthDisplay;
    }

    /// <summary>
    /// Can değiştiğinde çağrılır ve UI'ı günceller.
    /// </summary>
    /// <param name="currentHealth">Yeni can değeri.</param>
    private void UpdateHealthDisplay(int currentHealth)
    {
        // Can dizisi (hearts) sıfırdan başladığı için
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                // Can varsa kalbi göster
                hearts[i].enabled = true;
            }
            else
            {
                // Can yoksa kalbi gizle
                hearts[i].enabled = false;
            }
        }
    }
}