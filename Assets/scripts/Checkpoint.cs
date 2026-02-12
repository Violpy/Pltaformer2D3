using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем игроку, что это его новая точка возрождения
            other.GetComponent<PlayerController>().UpdateCheckpoint(transform.position);
            // Можно добавить звук или смену спрайта флага
        }
    }
}