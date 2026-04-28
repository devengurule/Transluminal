using Unity.VisualScripting;
using UnityEngine;

public class RatScript : MonoBehaviour
{
    [SerializeField] private float detectionRadius;
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;

    private AlienSaveData saveDataInstance;
    private EventManager eventManager;
    private Vector2 direction;
    private Rigidbody2D rb;
    private bool isFleeing;
    private Timer timer;
    private float duration = 2f;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        timer = gameObject.AddComponent<Timer>();
        timer.Initalize(duration, Dead, true);

        direction = GetRandomDirection();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Fleeing();
    }

    private void Dead()
    {
        eventManager.Publish(EventType.KillAlien, saveDataInstance);
        Destroy(gameObject);
    }

    public void Initialize(AlienSaveData saveDataInstance)
    {
        this.saveDataInstance = saveDataInstance;
    }

    private void Fleeing()
    {
        isFleeing = PlayerCollisionCheck(detectionRadius) ? true : false;

        if (isFleeing)
        {
            timer.Run();
            Move(direction, speed, maxSpeed);
        }
    }

    private Vector2 GetRandomDirection()
    {
        float randomRadAngle = Random.Range(30, 150f) * -Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(randomRadAngle), Mathf.Sin(randomRadAngle));
    }

    private void Move(Vector2 direction, Vector2 speed, Vector2 maxSpeed)
    {
        // Applies an impulse force to the rigidbody
        rb.AddForce(direction * speed * TimeManager.deltaTime, ForceMode2D.Impulse);

        // Truncates velocity to match the maximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxSpeed.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxSpeed.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxSpeed.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxSpeed.y;
    }

    private bool PlayerCollisionCheck(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D obj in colliders)
        {
            if (obj.gameObject.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction);
    }
    #endregion
}
