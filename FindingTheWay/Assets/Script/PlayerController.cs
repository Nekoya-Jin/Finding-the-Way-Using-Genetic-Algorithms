using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Score { get; private set; }
    
    public int[] DNA { get; set; }
    public float StartTime { get; set; }
    
    private GameManager _gameManager;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            CallScore(other.transform.name);
        }
        else if (other.CompareTag("Obstruction"))
        {
            Death();
        }
        else if (other.CompareTag("Wall"))
        {
            Death();
            // MoveToOppositeWall();
        }
    }

    //Timeで消える時に使う
    // private void MoveToOppositeWall()
    // {
    //     transform.Translate(2 * Vector3.left * _gameManager.PlayerSpeed * Time.deltaTime);
    // }
    
    private void Move()
    {
        if (DNA == null || _gameManager == null) return;

        var dnaIndex = (int)((Time.time - StartTime) / _gameManager.ActionInterval);

        if (dnaIndex >= DNA.Length)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        var currentGene = DNA[dnaIndex];
        var shouldMove = currentGene is >= 1 and <= 4;

        if (shouldMove)
        {
            var targetAngle = (currentGene % 4) * 90f;
            var targetRotation = Quaternion.Euler(0, targetAngle, 0);
            _rigidbody.MoveRotation(targetRotation);

            var moveDirection = transform.forward;
            _rigidbody.linearVelocity = moveDirection * _gameManager.PlayerSpeed;
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
    
    private void CallScore(string objectName)
    {
        if (int.TryParse(objectName, out var scoreValue))
        {
            Score = scoreValue;

            if (Score == GameManager.TargetScore)
            {
                _gameManager.Goal();
            }
        }
        else
        {
            Debug.LogWarning($"ObjectName：'{objectName}'はInt系にできません", gameObject);
        }
    }
    
    private void Death()
    {
        _gameManager.OnPlayerDeath();
        gameObject.SetActive(false);
    }
}
