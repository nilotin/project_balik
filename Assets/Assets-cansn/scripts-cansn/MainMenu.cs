using UnityEngine;
using UnityEngine.SceneManagement; // Scene yönetimi için gerekli

public class MainMenu : MonoBehaviour
{
    // Play butonuna týklandýðýnda çaðrýlacak metod
    public void PlayGame()
    {
        Debug.Log("PLAY button clicked");

        // Oyun sahnesine geçiþ
        SceneManager.LoadScene("PauseMenu"); // Sahne ismi yaz buraya gang
    }
}
