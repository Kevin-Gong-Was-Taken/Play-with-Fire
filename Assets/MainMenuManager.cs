using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string highScorePlayerprefSave;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [SerializeField] private GameObject audioMenu;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        highScoreText.text = $"Highscore: {PlayerPrefs.GetInt(highScorePlayerprefSave)}";

        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("soundeffectsVolume");
    }

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
        FindObjectOfType<AudioManager>().Play("Play");
    }

    public void LoadAudioMenu(bool b)
    {
        audioMenu.SetActive(b);
    }

    public void ChangeMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void ChangeSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat("soundeffectsVolume", volume);
    }
}
