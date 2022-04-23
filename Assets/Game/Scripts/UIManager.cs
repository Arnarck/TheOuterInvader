using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public AudioSource mainMenuMusic;
    public AudioSource[] inGameMusics;

    public Slider musicsSlider;
    public Slider soundEffectsSlider;
    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider bossHealthSlider;
    public Slider fuelSlider;

    public Text scoreText;
    public Text waveText;
    public Text finalScoreText;
    public Text finalWaveText;
    public Text completeScoreText;
    public Text completeWaveText;

    public Dropdown resolutionDropdown;

    public Gradient gradient;

    public GameObject playerAim;
    public GameObject MainMenu;
    public GameObject pauseMenuUI;
    public GameObject gameOverMenuUI;
    public GameObject levelCompleteMenuUI;
    public GameObject quitMenuUI;
    public GameObject shieldBar;
    public GameObject bossHealthBar;

    public bool GameIsPaused = false;

    public float slowMotionTime = 5f;

    public string whoWillDie;

    private AudioSource buttonSelectAudio;
    private SpawnManager spawnManager;
    private Resolution[] resolutions;

    private int score = 0;

    private bool canSlowMotion = false;

    private float currentTime = 0f;

    private void Start()
    {
        buttonSelectAudio = GetComponent<AudioSource>();
        buttonSelectAudio.volume = PlayerPrefs.GetFloat("SoundEffects");

        if (!PlayerPrefs.HasKey("Musics"))
        {
            PlayerPrefs.SetFloat("Musics", 1f);
        }

        if (!PlayerPrefs.HasKey("SoundEffects"))
        {
            PlayerPrefs.SetFloat("SoundEffects", 1f);
        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            musicsSlider.value = PlayerPrefs.GetFloat("Musics");
            soundEffectsSlider.value = PlayerPrefs.GetFloat("SoundEffects");

            mainMenuMusic.volume = PlayerPrefs.GetFloat("Musics");
            mainMenuMusic.Play();
            

            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        if (SceneManager.GetActiveScene().name == "Game")
        {
            inGameMusics[0].volume = PlayerPrefs.GetFloat("Musics");
            inGameMusics[1].volume = PlayerPrefs.GetFloat("Musics");
            inGameMusics[2].volume = PlayerPrefs.GetFloat("Musics");

            inGameMusics[0].Play();

            spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

            if (PlayerPrefs.GetInt("PlayerCurrentWave") >= 2 && PlayerPrefs.GetInt("PlayerCurrentWave") <= 10)
            {
                spawnManager.ChangeDifficulty((((float)PlayerPrefs.GetInt("PlayerCurrentWave") - 1f) / 20f) + 1f, (float)PlayerPrefs.GetInt("PlayerCurrentWave") / 10f - 0.1f);
            }
            else if (PlayerPrefs.GetInt("PlayerCurrentWave") > 10)
            {
                spawnManager.ChangeDifficulty(1.25f, 0.9f);
            }

            waveText.text = "ONDA ATUAL: " + PlayerPrefs.GetInt("PlayerCurrentWave");
        }
    }

    private void Update()
    {
        InputManager();

        if (canSlowMotion)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= slowMotionTime)
            {
                currentTime = 0f;
                canSlowMotion = false;

                if (whoWillDie.Equals("Player"))
                {
                    GameOver();
                }
                else if (whoWillDie.Equals("Boss"))
                {
                    LevelComplete();
                }
            }
        }
    }

    public void InputManager()
    {
        //A tela de "pause" só poderá ser ativada caso as telas de "game over" e "sair do jogo" estejam desativadas
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Game" && !gameOverMenuUI.activeInHierarchy && !quitMenuUI.activeInHierarchy && !levelCompleteMenuUI.activeInHierarchy)
        {
            //Se a tela de "pause" estiver ativa, e o jogador pressionar ESC, ela se desativa. E vice-versa
            if (GameIsPaused)
            {
                ResumeGame(pauseMenuUI);
            }
            else
            {
                PauseGame(pauseMenuUI);
            }
        }
        //A tela de "sair do jogo" só poderá ser ativada caso as telas de "game over" e "pause" estejam desativadas
        else if (Input.GetKeyDown(KeyCode.I) && SceneManager.GetActiveScene().name == "Game" && !gameOverMenuUI.activeInHierarchy && !pauseMenuUI.activeInHierarchy && !levelCompleteMenuUI.activeInHierarchy)
        {
            //Se a tela de "sair do jogo" estiver ativa, e o jogador pressionar I, ela se desativa. E vice-versa
            if (GameIsPaused)
            {
                ResumeGame(quitMenuUI);
            }
            else
            {
                PauseGame(quitMenuUI);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Menu")
        {
            if (!quitMenuUI.activeInHierarchy)
            {
                quitMenuUI.SetActive(true);
            }
            else
            {
                quitMenuUI.SetActive(false);
            }
        }
    }


    //=================MENU INTERFACES=================

    public void PlayGame(bool isRestartingGame)
    {
        buttonSelectAudio.Play();
        if (SceneManager.GetActiveScene().name == "Menu" || isRestartingGame)
        {
            PlayerPrefs.SetInt("PlayerCurrentScore", 0);
            PlayerPrefs.SetInt("PlayerCurrentWave", 1);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Game/Scenes/Game");
    }

    public void QuitGame()
    {
        buttonSelectAudio.Play();
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void CancelQuit()
    {
        buttonSelectAudio.Play();
        quitMenuUI.SetActive(false);
    }

    public void LoadMenu(GameObject ui)
    {
        ui.SetActive(true);
        buttonSelectAudio.Play();

    }

    public void UnLoadMenu(GameObject ui)
    {
        ui.SetActive(false);
        buttonSelectAudio.Play();
    }


    //==============MENU INTERFACE SETTERS===================

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("Musics", volume);
        mainMenuMusic.volume = volume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffects", volume);
        buttonSelectAudio.volume = volume;
    }


    //=================IN GAME INTERFACES=================
    public void PauseGame(GameObject ui)
    {
        buttonSelectAudio.Play();
        ui.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ResumeGame(GameObject ui)
    {
        buttonSelectAudio.Play();
        ui.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void BackToMenu()
    {
        buttonSelectAudio.Play();
        SceneManager.LoadScene("Game/Scenes/Menu");
    }

    public void UpdateScore(int updatedScore)
    {
        score += updatedScore;
        scoreText.text = "SCORE: " + score;
    }

    public void SlowMotion(string whoDied)
    {
        if (whoDied.Equals("Player"))
        {
            Destroy(playerAim.gameObject);
        }

        Time.timeScale = 0.5f;
        canSlowMotion = true;
        whoWillDie = whoDied;

    }

    public void GameOver()
    {
        if (inGameMusics[0].isPlaying)
        {
            inGameMusics[0].Stop();
        }
        else if (inGameMusics[1].isPlaying)
        {
            inGameMusics[1].Stop();
        }

        inGameMusics[2].Play();

        gameOverMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        finalScoreText.text = "PONTUAÇÃO FINAL: " + (PlayerPrefs.GetInt("PlayerCurrentScore") + score);
        finalWaveText.text = "ONDAS SOBREVIVIDAS: " + (PlayerPrefs.GetInt("PlayerCurrentWave") - 1);
    }

    public void LevelComplete()
    {
        if (inGameMusics[0].isPlaying)
        {
            inGameMusics[0].Stop();
        }
        else if (inGameMusics[1].isPlaying)
        {
            inGameMusics[1].Stop();
        }

        inGameMusics[2].Play();

        PlayerPrefs.SetInt("PlayerCurrentScore", PlayerPrefs.GetInt("PlayerCurrentScore") + score);
        PlayerPrefs.SetInt("PlayerCurrentWave", PlayerPrefs.GetInt("PlayerCurrentWave") + 1);

        levelCompleteMenuUI.SetActive(true);
        Time.timeScale = 0f;
        completeScoreText.text = "PONTUAÇÃO ATUAL: " + PlayerPrefs.GetInt("PlayerCurrentScore");
        completeWaveText.text = "ONDAS SOBREVIVIDAS: " + (PlayerPrefs.GetInt("PlayerCurrentWave") - 1);
    }


    //===================IN GAME Slider bars========================

    public void ActivateBossHealth(int healthPoints)
    {
        inGameMusics[0].Stop();
        inGameMusics[1].Play();

        bossHealthBar.SetActive(true);
        bossHealthSlider.maxValue = healthPoints;
        bossHealthSlider.value = healthPoints;
    }

    public void UpdateBossHealth(int healthPoints)
    {
        bossHealthSlider.value = healthPoints;
    }

    public void DeactivateBossHealth()
    {
        bossHealthSlider.value = 0;
        bossHealthBar.SetActive(false);
    }

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void SetMaxFuel(int fuel)
    {
        fuelSlider.maxValue = 100;
        fuelSlider.value = 100;
    }

    public void SetFuel(int fuel)
    {
        fuelSlider.value = fuel;
    }

    public void ActivateShield(int maxShieldPoints)
    {
        shieldBar.SetActive(true);
        shieldSlider.maxValue = maxShieldPoints;
        shieldSlider.value = maxShieldPoints;
    }

    public void UpdateShield(int shieldPoints)
    {
        shieldSlider.value = shieldPoints;
    }

    public void DeactivateShield()
    {
        shieldSlider.value = 0f;
        shieldBar.SetActive(false);
    }
}
