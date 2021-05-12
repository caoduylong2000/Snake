using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    private List<SnakeSegment> segments = new List<SnakeSegment>();
    private SnakeSegment head;
    public SnakeSegment segmentPrefab;
    public int initialSize = 4;

    private void Awake()
    {
        head = GetComponent<SnakeSegment>();

        if (head == null)
        {
            head = gameObject.AddComponent<SnakeSegment>();
            head.hideFlags = HideFlags.HideInInspector;
        }
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (head.direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                head.SetDirection(Vector2.up, Vector2.zero);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                head.SetDirection(Vector2.down, Vector2.zero);
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (head.direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                head.SetDirection(Vector2.right, Vector2.zero);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                head.SetDirection(Vector2.left, Vector2.zero);
            }
        }
    }

    private void FixedUpdate()
    {
        // Have each segment follow the one in front of it. We must do this in
        // reverse order so the position is set to the previous position,
        // otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].Follow(segments[i - 1], i, segments.Count);
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        float x = Mathf.Round(head.transform.position.x) + head.direction.x;
        float y = Mathf.Round(head.transform.position.y) + head.direction.y;

        head.transform.position = new Vector2(x, y);
    }

    public void Grow()
    {
        SnakeSegment segment = Instantiate(segmentPrefab);
        segment.Follow(segments[segments.Count - 1], segments.Count, segments.Count + 1);
        segments.Add(segment);
    }

    public void ResetState()
    {
        // Set the initial direction of the snake, starting at the origin
        // (center of the grid)
        head.SetDirection(Vector2.right, Vector2.zero);
        head.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list then add the head as the first segment
        segments.Clear();
        segments.Add(head);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food")) {
            Grow();
        } else if (other.gameObject.CompareTag("Obstacle")) {
            ResetState();
        }
    }

}
