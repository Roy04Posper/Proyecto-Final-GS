using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    public static MovablePlatform Instance;
    [Header("Configuración")]
    public float velocidad = 2f;

    [Tooltip("Puntos de destino (GameObjects vacíos)")]
    public Transform[] waypoints;



    private int indiceActual = 0;
    private int direccion = 1; // 1 para adelante, -1 para atrás

    void Update()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Transform destino = waypoints[indiceActual];

        // Mover la plataforma hacia el waypoint actual
        transform.position = Vector3.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);

        // Si llega al waypoint, cambiar al siguiente según la dirección
        if (Vector3.Distance(transform.position, destino.position) < 0.01f)
        {
            indiceActual += direccion;

            // Cambiar de dirección si llegamos al final o al inicio
            if (indiceActual >= waypoints.Length)
            {
                indiceActual = waypoints.Length - 2;
                direccion = -1;
            }
            else if (indiceActual < 0)
            {
                indiceActual = 1;
                direccion = 1;
            }
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            collision.gameObject.transform.parent = null;
        }
    }

    // Mostrar gizmos en el editor
    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}

