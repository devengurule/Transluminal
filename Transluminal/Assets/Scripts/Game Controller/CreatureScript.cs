using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatureScript : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;

    private Rigidbody2D rb;
    private AlienSaveData saveDataInstance;
    private Vector2 direction = Vector2.right;
    
    private void Start()
    {
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
}
