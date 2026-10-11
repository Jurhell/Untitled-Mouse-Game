using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : MonoBehaviour
{
    [SerializeField, Tooltip("The UI canvas for the ending screen.")]
    private GameObject _endScreenUI;

    [SerializeField, Tooltip("The UI Prefab")]
    private GameObject _ui;

    private bool _isCalled;

    private void Awake()
    {
        _isCalled = false;
        _endScreenUI.SetActive(false);
    }

    public void EndingCall()
    {
        if (_isCalled || !GameplayManager.GameOver)
            return;

        _isCalled = true;

        //Enable the end screen UI and disable the main UI
        _endScreenUI.SetActive(true);
        _ui.SetActive(false);

        Time.timeScale = 0f;
    }

    public void RestartGame() => GameplayManager.OnReset();
    public void MainMenu() => GameplayManager.OnMenu();
    public void QuitGame() => GameplayManager.OnQuit();
}
