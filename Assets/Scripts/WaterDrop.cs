using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    [SerializeField] private string gameOverReason = "waterDrop";
    [SerializeField] private int oilScore;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Triggers Game Over if it hits the player and he doesn't have the oil effect
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player.hasOil)
            {
                FindObjectOfType<GameManager>().IncreaseScore(oilScore);
                player.ResetHealth();

                FindObjectOfType<AudioManager>().Play("Water Pickup");
            }
            else
            {

                FindObjectOfType<GameManager>().GameOver(gameOverReason);
            }
        }

        if (collision.CompareTag("Player") || collision.CompareTag("DeathCollider"))
        {
            Destroy(gameObject);
        }
    }
}
