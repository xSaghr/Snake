using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    public AudioSource audioPlayer;
    private Snake snake;
    public int foodCount = 0;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
        RandomizePosition();
    }

    private void RandomizePosition()
    {
        Bounds bounds = this.gridArea.bounds;

        //Random position within bounds + rounding
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the food from spawning on the snake
        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y)
                {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }

    public bool OccupiesFood(int x, int y)
    {
        
        if (Mathf.RoundToInt(transform.position.x) == x &&
            Mathf.RoundToInt(transform.position.y) == y)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //audioPlayer.PlayOneShot(audioPlayer);
            audioPlayer.Play();
            RandomizePosition();
            foodCount++;
            //GameObject.FindGameObjectWithTag("Food").transform.position = food.transform.position;
        }
    }
}
