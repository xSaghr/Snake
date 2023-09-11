using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PowerUp : MonoBehaviour
{
    public BoxCollider2D gridArea;
    public AudioSource audioPlayer;
    private Snake snake;
    public Food food;
    public int powerUpFoodCount = 2;
    public bool powerUpSpawned = false;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        Bounds bounds = this.gridArea.bounds;

        //Random position within bounds + rounding
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the food from spawning on the snake
        while (snake.Occupies(x, y) && food.OccupiesFood(x, y))
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

        powerUpSpawned = true;

        food.foodCount = 0;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //audioPlayer.PlayOneShot(audioPlayer);
            audioPlayer.Play();
            
            if (food.foodCount < powerUpFoodCount)
            {
                this.transform.position = new Vector3(0.0f, 25.0f, 0.0f);
                powerUpSpawned = false;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (food.foodCount == powerUpFoodCount)
        {
            SpawnPowerUp();
        }
        //GameObject.FindGameObjectWithTag("Food").transform.position = food.transform.position;
    }
}
