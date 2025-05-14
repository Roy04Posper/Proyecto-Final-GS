using Unity.VisualScripting;
using UnityEngine;

public class SavePointController : MonoBehaviour
{
   public static SavePointController Instance;
   public Transform respawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            collision.transform.position = respawnPoint.position;
            SaveSystem.Save();
        }
    }
    public void Respawn()
    {
        PlayerMovement.Player.gameObject.SetActive(false);
        PlayerMovement.Player.transform.position = respawnPoint.position;
        PlayerMovement.Player.gameObject.SetActive(true);
    }
}
