using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    private Rigidbody2D rb;
    private bool isGrounded;


    public float jumpPower = 2.0f;
    public float playerHearts = 3.0f;

    public AudioSource jumpSound;
    public AudioSource coinSound;


    private void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
        }
    }




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Initialize the jumpPower from GameManager at the start
        jumpPower = GameManager.instance.jumpPower;
    }


    void Update()
    {
        // Sync jumpPower with GameManager
        jumpPower = GameManager.instance.jumpPower;

        Movement();
    }

    private void Movement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        float moveSpeed = GameManager.instance.playerSpeed; // Get speed from GameManager
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Flip to left
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Flip to right
        }
    }

    void Jump()
    {
        jumpSound.Play();
        rb.velocity = new Vector2(rb.velocity.x, jumpPower); // Use the updated jumpPower
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            playerHearts--;

            if (playerHearts <= 0)
            {
                GameManager.instance.YouLost();
            }
            else
            {
                GameManager.instance.playerTime = 0;
                GameManager.instance.GoToShop();
            }

            

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for coins
        if (collision.CompareTag("Coin"))
        {
            coinSound.Play();
            GameManager.instance.AddCoins(1); // Add coins via GameManager

            // Return the coin to the pool instead of destroying it
            CoinPool coinPool = FindObjectOfType<CoinPool>();
            coinPool.ReturnCoin(collision.gameObject); // Return coin to pool
            Debug.Log("Coin Collected!");
        }
        else if (collision.CompareTag("Tutorial Coin"))
        {
            coinSound.Play();
            CoinPool coinPool = FindObjectOfType<CoinPool>();
            coinPool.ReturnCoin(collision.gameObject); // Return tutorial coin to pool
            GameManager.instance.StartGame(); // Start the game when tutorial coin is collected
            Debug.Log("Tutorial Coin Collected!");
        }
        
    }

}
