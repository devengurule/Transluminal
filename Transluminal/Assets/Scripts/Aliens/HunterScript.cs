using Unity.VisualScripting;
using UnityEngine;

public class HunterScript : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private Vector2 acceleration;
    [SerializeField] private Vector2 maxVelocity;

    private EventManager eventManager;
    private Rigidbody2D rb;

    private Vector2 direction = Vector2.one;
    private bool movingRight = true;
    private bool movingUp = true;
    private RaycastHit2D ray;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        rb = GetComponent<Rigidbody2D>();

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.KillAlien, Dead);
            eventManager.Subscribe(EventType.Pause, (object target) => rb.linearVelocity = Vector2.zero);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.KillAlien, Dead);
            eventManager.Unsubscribe(EventType.Pause, (object target) => rb.linearVelocity = Vector2.zero);
        }
    }

    private void Update()
    {
        //AI logic
        direction.x = movingRight ? 1 : -1;
        direction.y = movingUp ? 1 : -1;

        ray = Physics2D.Raycast(transform.position, Vector2.right * direction.x, distance, LayerMask.GetMask("Collision"));

        if(ray.collider != null)
        {
            FlipXDirection();
        }

        rb.AddForce(direction * acceleration * TimeManager.deltaTime, ForceMode2D.Impulse);

        if (Mathf.Abs(rb.linearVelocityX) > maxVelocity.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxVelocity.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxVelocity.y)
        {
            rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxVelocity.y;
            FlipYDirection();
        }
    }

    private void Dead(object target)
    {
        if(target is GameObject alien && alien == gameObject)
        {
            Destroy(gameObject);
        }
    }

    private void FlipXDirection()
    {
        movingRight = !movingRight;
    }
    private void FlipYDirection()
    {
        movingUp = !movingUp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(transform.position, Vector2.right * direction.x * distance);
    }
}
