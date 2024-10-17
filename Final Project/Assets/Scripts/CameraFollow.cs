using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    public Transform player1; // Assign player 1 in the Inspector
    public Transform player2; // Assign player 2 in the Inspector
    public float distance = 5f; // Distance to maintain from the players
    public float smoothSpeed = 0.125f; // Smoothness factor for camera movement
    public float verticalThreshold = 2f; // Height threshold for vertical movement
    public float horizontalThreshold = 2f; // Horizontal threshold for left/right movement

    private Vector3 targetPosition;

    private void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager
        }
    }

    public void ResetCamera()
    {
        // Set the camera's starting position
        transform.position = new Vector3(0.48f, -3.88f, -111.84f);
    }

    private void Start()
    {
        // Set the camera's starting position
        transform.position = new Vector3(0.48f, -3.88f, -111.84f);
    }

    private void LateUpdate()
    {
        // Calculate the average position of the players
        Vector3 averagePosition = (player1.position + player2.position) / 2;

        // Determine the target position based on average position
        targetPosition = new Vector3(averagePosition.x, transform.position.y, transform.position.z);

        // Determine if the camera needs to move vertically (up or down)
        if (averagePosition.y > transform.position.y + verticalThreshold)
        {
            targetPosition.y = averagePosition.y + distance; // Move up if players go above the threshold
        }
        else if (averagePosition.y < transform.position.y - verticalThreshold)
        {
            targetPosition.y = averagePosition.y - distance; // Move down if players go below the threshold
        }


        // Determine if the camera needs to move horizontally
        if (averagePosition.x < transform.position.x - horizontalThreshold)
        {
            targetPosition.x = averagePosition.x; // Move left if players go left of the threshold
        }
        else if (averagePosition.x > transform.position.x + horizontalThreshold)
        {
            targetPosition.x = averagePosition.x; // Move right if players go right of the threshold
        }



        // Smoothly interpolate the camera's position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    private void FixedUpdate()
    {
        Vector3 averagePosition = (player1.position + player2.position) / 2;
        targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (averagePosition.y > transform.position.y + verticalThreshold)
        {
            targetPosition.y = averagePosition.y + distance;
        }
        else if (averagePosition.y < transform.position.y - verticalThreshold + distance)
        {
            targetPosition.y = averagePosition.y - distance;
        }

        if (averagePosition.x < transform.position.x - horizontalThreshold)
        {
            targetPosition.x = averagePosition.x;
        }
        else if (averagePosition.x > transform.position.x + horizontalThreshold)
        {
            targetPosition.x = averagePosition.x;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

}
