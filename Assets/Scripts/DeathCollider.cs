using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    private GameManager gm;
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>(); 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gm.GameOver("fall");
        }
    }
}
