using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Collider2D trigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.CompareTag("Player"))
        {
            SavePointController.Instance.respawnPoint = transform;
            trigger.enabled = false;
        }
    }
}
