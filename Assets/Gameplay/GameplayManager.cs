using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private static GameplayManager _instance;
    public static GameplayManager Instance => _instance;

    private static float _playerHealth = 3f;
    private static float _healthReset;

    public static bool _bGameStarted = false;
    private static bool _bGameOver = false;
    private static bool _bPlayerIsInvincible = false;

    public static bool GameOver => _bGameOver;
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

        _playerHealth -= 1f;

        PlayerInvincibilty();

        if (_playerHealth <= 0f)
            TriggerEndState();
    }

    private static void ResetHealth() => _playerHealth = _healthReset;

    private static void PlayerInvincibilty() => _bPlayerIsInvincible = true;
    private static void EndInvincibilty() => _bPlayerIsInvincible = false;

    public static void TriggerEndState()
    {
        _bGameOver = false;
    }

    public static void OnReset()
    {
        ResetHealth();
    }
}
