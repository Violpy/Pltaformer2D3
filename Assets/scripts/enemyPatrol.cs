using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public Transform point1, point2;
    
    [Header("Detection")]
    public float detectionRange = 5f; // Радиус обнаружения
    public Transform player; // Сюда перетащи объект Player в инспекторе

    private Transform target;
    private bool isChasing = false;

    void Start() 
    {
        target = point1;
        // Если забыла перетащить игрока в инспекторе, скрипт попытается найти его сам
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;

        // Логика переключения состояний
        if (distanceToPlayer < detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange * 1.5f) // Перестает гнаться, если игрок убежал далеко
        {
            isChasing = false;
        }

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        FlipSprite();
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            target = target == point1 ? point2 : point1;
        }
    }

    void ChasePlayer()
    {
        // Враг бежит за игроком
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y); // Бежит только по горизонтали
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
    }

    void FlipSprite()
    {
        // Определяем направление на основе того, гнались мы или патрулировали
        float targetX = isChasing ? player.position.x : target.position.x;

        if (targetX > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // Рисуем радиус в редакторе для удобства
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}