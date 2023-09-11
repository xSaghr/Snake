using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    //Declaring objects
    public Vector2Int direction = Vector2Int.right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;

    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;

    private Vector2Int input;
    private float nextUpdate;

    //Objects to add at start of game
    private void Start()
    {
            ResetState();
    }

    //Snake head movement positioning
    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                input = Vector2Int.up;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                input = Vector2Int.right;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                input = Vector2Int.left;
            }
        }
    }
    //active updating of objects
    private void FixedUpdate()
    {

        // Wait for next update to continue
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero)
        {
            direction = input;
        }
        //Snake tail movement positioning back to front
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        //object new position rounding to keep it grid centred
        int x = Mathf.RoundToInt(this.transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(this.transform.position.y) + direction.y;

        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));

    }

    //Snake growing feature
    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private IEnumerator PowerUp()
    {
        //change speed to half
        speedMultiplier = 0.5f;
        yield return new WaitForSeconds(10);
        //change speed back to normal
        speedMultiplier = 1f;
    }

    private void ResetState()
    {

        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        //i = 1 to skip the head from getting destroyed
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        //clear all and add back head
        _segments.Clear();
        _segments.Add(this.transform);

        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
            //old way: _segments.Add(Instantiate(this.segmentPrefab));
        }

        this.transform.position = Vector3.zero;
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform _segment in _segments)
        {
            if (Mathf.RoundToInt(_segment.position.x) == x &&
                Mathf.RoundToInt(_segment.position.y) == y)
            {
                return true;
            }
        }

        return false;
    }

    //Collision detect what object it collides with
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        } 
        else if (other.tag == "Obstacle")
        {
            ResetState();
        }
        else if (other.tag == "PowerUp")
        {
            StartCoroutine(PowerUp());
        }
        else if (other.tag == "Wall")
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                ResetState();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }
}
