using UnityEngine;
using UnityEngine.SceneManagement; // Scene y�netimi i�in gerekli

public class MainMenu : MonoBehaviour
{
    // Play butonuna t�kland���nda �a�r�lacak metod
    public void PlayGame()
    {
        Debug.Log("PLAY button clicked");

        // Oyun sahnesine ge�i�
        SceneManager.LoadScene("SampleScene"); // Sahne ismi yaz buraya gang
    }
}
