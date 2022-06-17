using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using TMPro;
public class GameManager : MonoBehaviour
{
    private Player player;

    [Header("Game Over Menu")]
    [Space]
    [SerializeField] private List<DeathMessageKey> deathMessages = new List<DeathMessageKey>();
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private TextMeshProUGUI deathMessageText;
    [SerializeField] private TextMeshProUGUI scoreGameOverText;
    [SerializeField] private TextMeshProUGUI highscoreGameOverText;


    private bool gameOver = false;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string highScorePPSave;

    [SerializeField] private List<Drop> drops = new List<Drop>();
    [SerializeField] private Transform coinSpawnposition;
    [SerializeField] private float spawnWidth;

    [SerializeField] private int score;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private float originalSpawnTimeCooldown;
    private float spawnTimeCooldown;

    [Header("Power Up Stuff")]
    // mostly references for the power up scripts

    [SerializeField] private Light2D globalLight;

    [SerializeField] private float increasedIntensity = .7f;
    private float originalIntensity;
    [SerializeField] private float intensityReducingMultiplier;

    private bool hasIncreasedIntensity;
    private void Start()
    {
        scoreText.text = $"Score: {score}";
        gameOverMenu.SetActive(false);
        player = FindObjectOfType<Player>();

        originalIntensity = globalLight.intensity;
    }
    public void LightUp()
    {
        globalLight.intensity = increasedIntensity;
        hasIncreasedIntensity = true;
    }
    private void Update()
    {
        spawnTimeCooldown -= Time.deltaTime;
        if (spawnTimeCooldown < 0)
        {
            SpawnDrop();
            spawnTimeCooldown = originalSpawnTimeCooldown;
        }

        if (hasIncreasedIntensity)
        {
            globalLight.intensity -= Time.deltaTime * intensityReducingMultiplier;
            hasIncreasedIntensity = globalLight.intensity > originalIntensity;

            if (!hasIncreasedIntensity)
            {
                globalLight.intensity = originalIntensity;
            }
        }
    }

    private void SpawnDrop()
    {
        // Gets a drop from the drops list based of its weight, the higher the weight, the more often it gets added to the weightedDrops List

        List<Drop> weightedDrops = new List<Drop>();

        foreach (var item in drops)
        {
            for (int i = 0; i < item.weight; i++)
            {
                weightedDrops.Add(item);
            }
        }

        // Spawns the Coin
        Instantiate(weightedDrops[Random.Range(0, weightedDrops.Count)].droppingItem, coinSpawnposition.transform.position + new Vector3(Random.Range(-spawnWidth, spawnWidth), 0, 0), Quaternion.identity);

    }
    /// <summary>
    /// Increases the score and updates the score text
    /// </summary>
    /// <param name="addedScore">the amount the score is getting increased by</param>
    public void IncreaseScore(int addedScore)
    {
        score += addedScore * (player.HasSpeedBoost() ? 2 : 1);

        scoreText.text = $"Score: {score}";
    }

    /// <summary>
    /// Activates the Game Over Screen and destroys the Player 
    /// </summary>
    public void GameOver()
    {
        if (gameOver) return;

        Destroy(player.gameObject);

        gameOverMenu.SetActive(true);

        deathMessageText.text = FindDeathMessage("default");

        gameOver = true;

        if (PlayerPrefs.GetInt(highScorePPSave) < score)
        {
            PlayerPrefs.SetInt(highScorePPSave, score);
        }

        scoreGameOverText.text = $"Score: {score}";
        scoreGameOverText.text = $"Score: {PlayerPrefs.GetInt(highScorePPSave)}";

        FindObjectOfType<AudioManager>().Play("Death");
    }
    /// <summary>
    /// Activates the Game Over Screen and destroys the Player 
    /// </summary>
    /// <param name="deathReason">The reason of the death of the player</param>
    public void GameOver(string deathReason)
    {
        if (gameOver) return;

        Destroy(FindObjectOfType<Player>().gameObject);

        gameOverMenu.SetActive(true);

        deathMessageText.text = FindDeathMessage(deathReason);

        gameOver = true;

        if (PlayerPrefs.GetInt(highScorePPSave) < score)
        {
            PlayerPrefs.SetInt(highScorePPSave, score);
        }

        scoreGameOverText.text = $"Score: {score}";
        highscoreGameOverText.text = $"Highscore: {PlayerPrefs.GetInt(highScorePPSave)}";

        FindObjectOfType<AudioManager>().Play("Death");
    }
    private string FindDeathMessage(string reason)
    {
        string msg;
        try
        {
            msg = deathMessages.Find(x => x.reason == reason).message;
        }
        catch (System.Exception)
        {
            msg = "There was an error trying to find the right death message";
        }

        return msg;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(coinSpawnposition.position, new Vector3(spawnWidth * 2, .5f));
    }
    /// <summary>
    /// Reloads the active Scene
    /// </summary>
    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// Loads the Main Menu Scene
    /// </summary>
    public void GoHome()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    [System.Serializable]
    public struct DeathMessageKey
    {
        public string reason;
        [TextArea(3, 8)]
        public string message;
    }
    [System.Serializable]
    public class Drop
    {
        public GameObject droppingItem;
        public int weight = 1;
    }
}
