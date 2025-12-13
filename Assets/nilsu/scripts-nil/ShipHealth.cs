using UnityEngine;
using UnityEngine.SceneManagement; // Sahneyi yeniden yüklemek için

public class ShipHealth : MonoBehaviour
{
    // GameManager'dan almak yerine, başlangıç canını burada tutalım.
    [Header("Health Settings")]
    [Tooltip("Geminin başlangıçtaki maksimum canı.")]
    public int maxHealth = 3; 

    private int currentHealth;

    // UI Güncellemesi için delegate/event kullanmak idealdir,
    // ancak şimdilik basit bir metod üzerinden gidebiliriz.
    public delegate void HealthChange(int newHealth);
    public static event HealthChange OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        // UI'ın başlangıç canını göstermesi için bir kez tetikle
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Gemiye hasar verir ve canı kontrol eder.
    /// </summary>
    /// <param name="damage">Alınan hasar miktarı.</param>
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Zaten ölmüşse hasar almasın.

        currentHealth -= damage;
        
        // UI'ı bilgilendir
        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log($"Gemi hasar aldı! Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Gemi batırıldı! Oyun Bitti.");
        
        // Buraya "Game Over" ekranı veya sahneyi yeniden yükleme kodu gelebilir.
        // Basitçe sahneyi yeniden yüklüyoruz:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Şimdilik sadece objeyi devre dışı bırakalım.
        // gameObject.SetActive(false); 
        
        // Oyunu tamamen durdurmak ve görmek için
        Time.timeScale = 0f;
    }
    
    // Test veya Power-up'lar için can yenileme
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth);
    }
}