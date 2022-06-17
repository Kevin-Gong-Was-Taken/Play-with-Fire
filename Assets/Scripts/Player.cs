using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    private LineRenderer lr;
    private TrailRenderer tr;
    private Light2D playerLight;
    private Color defaultLightColor;
    [SerializeField] private ParticleSystem oilParticle;
    [SerializeField] private Transform dragTracker;
    [SerializeField] private GameObject postProcessing;
    [SerializeField] private Slider healthSlider;

    [Header("Dragging")]

    [SerializeField] private float forceMultiplicator;
    [SerializeField] private float arrowLength;

    private bool aiming = false;

    [Space]

    [SerializeField] private float increasedSpeed = 100f;
    [SerializeField] private float speedIncreaseDuration = 4f;
    private float speedIncreaseTimer = 0;
    // the current speed boost of the player
    public float activeSpeedBoost
    {
        get { return HasSpeedBoost() ? increasedSpeed : 0; }
    }

    [Header("Aiming")]

    [Space]
    [SerializeField] private float aimingHealthMultiplier;
    [SerializeField] private float aimingTimeReducer;

    // Fields for calculating the gravity scale of the Player
    [Header("Gravity")]
    [SerializeField] private float baseGravity = .5f;
    [SerializeField] private float gravityFactor = .6f;
    [SerializeField] private float gravityDivider = 18;

    private float elapsedTime = 0f;
    // The calculated gravity uses the following formular: g = c+b*sqrt(t/a)
    // a = gravityDivider; b = gravityFactor; c= baseGravity and t is the elapsed time in seconds

    [Header("Timescale")]
    [SerializeField] private float timeIncrement = .001f;
    [SerializeField] private float startTime;
    [SerializeField] private float maxTime;

    private float activeTimeScale;


    [Header("Health System")]

    private float health;
    [SerializeField] private float maxHealth = 7f;

    [Header("Oil")]
    [SerializeField] private float oilDuration;
    [SerializeField] private Color oilColor;
    private float oilTimer;
    public bool hasOil { get { return oilTimer > 0; } }

    // Start is called before the first frame update
    void Start()
    {
        // Assigns the References
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        tr = GetComponent<TrailRenderer>();
        playerLight = transform.GetComponentInChildren<Light2D>();
        defaultLightColor = playerLight.color;

        activeTimeScale = startTime;
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
    }

    private void Awake()
    {
        postProcessing.SetActive(false);
    }

    /// <summary>
    /// activates the oil effect
    /// </summary>
    public void ActivateOil()
    {
        oilTimer = oilDuration;
        oilParticle.Play();
    }
    private float GetGravityScale()
    {
        return baseGravity + (gravityFactor * Mathf.Sqrt(elapsedTime / gravityDivider));
    }
    private void FixedUpdate()
    {
        rb.gravityScale = GetGravityScale();
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }
    /// <summary>
    /// Boosts the speed of the player by a set amount of time and activates the trail Renderer
    /// </summary>
    public void SpeedBoost()
    {
        speedIncreaseTimer = speedIncreaseDuration;
        HasSpeedBoost();
    }

    public bool HasSpeedBoost()
    {
        return speedIncreaseTimer >= 0;
    }

    // Update is called once per frame
    void Update()
    {

        tr.enabled = HasSpeedBoost();

        // Reduces health by deltaTime and if the player is aiming, decrease it by aimingHealthmultiplier * (1/aimingTimeReducer) * deltaTime (aimingHealthMultiplier times the speed at which it normally would decrease)
        health -= (aiming ? aimingHealthMultiplier * (1 / aimingTimeReducer) : 1) * Time.deltaTime;
        healthSlider.value = health;

        // Game Speed
        speedIncreaseTimer -= Time.deltaTime;

        // Oil
        oilTimer -= Time.deltaTime;

        if (oilTimer < .75f)
        {
            oilParticle.Stop();
            playerLight.color = defaultLightColor;
        }
        else
        {
            playerLight.color = oilColor;
        }

        if (health < 0)
        {
            FindObjectOfType<GameManager>().GameOver("burntOut");
            return;
        }


        elapsedTime += Time.deltaTime;

        if (activeTimeScale < maxTime)
        {
            activeTimeScale += timeIncrement * Time.deltaTime;
        }
        else
        {
            activeTimeScale = maxTime;
        }

        // sets the timeScale to the scale of right now. The if the player is aiming, the timeScale is a forth of the actual timeScale
        Time.timeScale = aiming ? aimingTimeReducer * activeTimeScale : activeTimeScale;

        // gets the position of the mouse in the world space
        Vector2 mousePos = FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);


        // gets the position of the first click and sets it to the dragTrackers position
        if (Input.GetButtonDown("Fire1"))
        {
            aiming = true;
            dragTracker.position = mousePos;
            postProcessing.SetActive(true);
        }
        // draws a line from the gameObject to direction of the Vector -(mousePos -dragTracker.position), since the player gets launched into the opposite direction and the dragTrackers position, so its not dependend to the position of the Camera
        if (Input.GetButton("Fire1"))
        {
            lr.SetPosition(0, (Vector2)transform.position);
            lr.SetPosition(1, (Vector2)transform.position - (mousePos - (Vector2)dragTracker.position).normalized * arrowLength);
        }
        // When the Player releases the mouse Button, launch him into the opposite direction of the mouse
        if (Input.GetButtonUp("Fire1"))
        {
            aiming = false;
            rb.velocity = new Vector2(0, 0);

            // Adds Force to the Player in the opposite direction of the mouse
            rb.AddForce(-(mousePos - (Vector2)dragTracker.position).normalized * (forceMultiplicator + activeSpeedBoost));

            // Resets all viusal effects
            lr.SetPositions(new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) });
            postProcessing.SetActive(false);

            FindObjectOfType<AudioManager>().Play("Move");
        }

    }
}
