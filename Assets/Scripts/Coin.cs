using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value;
    private void OnPickup()
    {
        Destroy(gameObject);
        FindObjectOfType<GameManager>().IncreaseScore(value);
        FindObjectOfType<Player>().ResetHealth();

        FindObjectOfType<AudioManager>().Play("Score Increment");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the collision is a Player
        if (collision.CompareTag("Player"))
        {
            OnPickup();
        }
        if (collision.CompareTag("Player") || collision.CompareTag("DeathCollider"))
        {
            Destroy(gameObject);
        }
    }
}
