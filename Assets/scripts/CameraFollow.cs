using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Ссылка на игрока
    public float smoothness = 0.125f; // Насколько плавно идет камера (чем меньше, тем медленнее)
    public Vector3 offset = new Vector3(0, 2, -10); // Смещение камеры относительно игрока

    void LateUpdate() // LateUpdate лучше для камеры, так как выполняется после движения игрока
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            // Плавный переход между текущей позицией и позицией игрока
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothness);
            transform.position = smoothedPosition;
        }
    }
}