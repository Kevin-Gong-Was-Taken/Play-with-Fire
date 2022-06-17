using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour
{
    public UnityEvent OnPickUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickUp.Invoke();
            FindObjectOfType<AudioManager>().Play("Power Up");
            Destroy(gameObject);
        }
    }


    // Methods for the different PowerUps 

    public void LightUp()
    {
        FindObjectOfType<GameManager>().LightUp();
    }

    public void SpeedBoost()
    {
        FindObjectOfType<Player>().SpeedBoost();
    }

    public void ActivateOil()
    {
        FindObjectOfType<Player>().ActivateOil();
    }

    public void IncreaseScore()
    {
        FindObjectOfType<GameManager>().IncreaseScore(5);
        FindObjectOfType<Player>().ResetHealth();

        //FindObjectOfType<AudioManager>().Play("Coal");
    }
}
