using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatureScript : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;

    private Rigidbody2D rb;
    private AlienSaveData saveDataInstance;
    private Vector2 direction = Vector2.right;
    private EventManager eventManager;
    
    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move(direction, speed, maxSpeed);
    }

    private void OnDestroy()
    {
        if (GameController.instance != null)
        {
            GameController.instance.GetComponent<AlienManager>().SetCreaturePosition(saveDataInstance, transform.position);
        }
    }

    public void Initialize(AlienSaveData data)
    {
        saveDataInstance = data;
    }

    private void Move(Vector2 direction, Vector2 speed, Vector2 maxSpeed)
    {
        // Applies an impulse force to the rigidbody
        rb.AddForce(direction * speed * TimeManager.deltaTime, ForceMode2D.Impulse);

        // Truncates velocity to match the maximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxSpeed.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxSpeed.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxSpeed.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxSpeed.y;
    }

    private void DealDamage()
    {
        Vector2Int damage = GameController.instance.GetComponent<AlienManager>().CreatureDamageRange();
        GameController.instance.GetComponent<HealthManager>().SubtractHealth(Random.Range(damage.x, damage.y + 1));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            DealDamage();
            eventManager.Publish(EventType.AttackPlayer, gameObject);
        }
    }
}
