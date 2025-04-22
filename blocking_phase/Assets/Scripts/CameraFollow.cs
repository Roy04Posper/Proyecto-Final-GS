using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo a Seguir")]
    [SerializeField] private Transform target;

    [Header("Configuración de Seguimiento")]
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

    private float initialZ;

    private void Start()
    {
        initialZ = transform.position.z;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = initialZ; // Mantiene la cámara en el plano 2D

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

