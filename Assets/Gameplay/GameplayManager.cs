using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    private static GameplayManager _instance;
    public static GameplayManager Instance => _instance;

    private static int _playerHealth = 3;
    private static int _healthReset;

    public static bool _bGameStarted = false;
    private static bool _bGameOver = false;
    private static bool _bGamePaused = false;
    private static bool _bPlayerIsInvincible = false;

    public static int PlayerHealth => _playerHealth;
    public static bool GameOver => _bGameOver;
    public static bool GamePaused => _bGamePaused;
    public static bool Invincible => _bPlayerIsInvincible;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
        DontDestroyOnLoad(gameObject);

        _healthReset = _playerHealth;
    }

    public static void bGameStarted() => _bGameStarted = true;

    public static void DamagePlayer()
    {
        if (_bPlayerIsInvincible)
            return;

        _playerHealth -= 1;

        PlayerHealthUI playerHealthUI = FindObjectOfType<PlayerHealthUI>();
        playerHealthUI.UpdateHealth();

        PlayerInvincibilty();

        if (_playerHealth <= 0)
            TriggerEndState();
    }

    private static void ResetHealth() => _playerHealth = _healthReset;

    private static void PlayerInvincibilty() => _bPlayerIsInvincible = true;
    public static void EndInvincibilty() => _bPlayerIsInvincible = false;

    public static void TriggerEndState()
    {
        _bGameOver = true;
    }

    public static void OnReset()
    {
        ResetHealth();

        //Restoring game time and loading the current scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void OnPause()
    {
        _bGamePaused = true;
        //Freezing the game world
        Time.timeScale = 0.0001f;
    }

    public static void OnUnPause()
    {
        _bGamePaused = false;
        //Unfreezing the game world
        Time.timeScale = 1f;
    }

    public static void OnQuit()
    {
        //Restoring game time and loading the main menu scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
