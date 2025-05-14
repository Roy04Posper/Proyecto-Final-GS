using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerOnCollision : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string nombreDeEscena;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(nombreDeEscena))
            {
                SceneManager.LoadScene(nombreDeEscena);
            }
            else
            {
                Debug.LogWarning("No se ha asignado un nombre de escena en el Inspector.");
            }
        }
    }
}

