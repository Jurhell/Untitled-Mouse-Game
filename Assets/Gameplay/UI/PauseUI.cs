using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField, Tooltip("The pause menu.")]
    private GameObject _pauseMenuUI;

    [SerializeField, Tooltip("The options menu.")]
    private GameObject _optionsMenuUI;

    [Header("UI Buttons")]
    [SerializeField, Tooltip("The resume button for debugging.")]
    private GameObject _resumeButton;

    [SerializeField, Tooltip("The options button for debugging.")]
    private GameObject _optionsButton;

    private PlayerControls _playerActions;

    private InputAction _menu;

    public UnityEvent OnPause;
    public UnityEvent OnUnPause;

    private void Awake()
    {
        //Creating Player Controller
        _playerActions = new PlayerControls();
        _pauseMenuUI.SetActive(false);
        _optionsMenuUI.SetActive(false);
    }

    //private void OnEnable()
    //{
    //    //Storing and enabling pause action
    //    _menu = _playerActions.Menu.Pause;
    //    _menu.Enable();
        
    //    _menu.performed += MenuCall;
    //}

    //private void OnDisable() => _menu.Disable();

    //public void MenuCall(InputAction.CallbackContext context)
    //{
    //    //Only running once while button is pressed
    //    if (!context.started)
    //        return;

    //    //Check if game is already paused
    //    if (GameplayManager.GamePaused)
    //        Resume();
    //    else
    //        Pause();
    //}

    public void MenuCall()
    {
        //Check if game is already paused
        if (GameplayManager.GamePaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        //Activating pause menu
        _pauseMenuUI.SetActive(true);
        //Deselecting the resume button to avoid it being selected when the pause menu is opened
        _resumeButton.GetComponent<Button>().OnDeselect(null);

        GameplayManager.OnPause();

        OnPause.Invoke();
    }

    //Unpauses the game
    public void Resume()
    {
        //Deactivating pause and options menu
        _pauseMenuUI.SetActive(false);
        _optionsMenuUI.SetActive(false);

        GameplayManager.OnUnPause();

        OnUnPause.Invoke();
    }

    //Reloads the current scene
    public void Retry()
    {
        GameplayManager.OnReset();

        OnUnPause.Invoke();
    }

    public void OptionsMenu()
    {
        //Enabling the options menu and disabling the pause menu
        _optionsMenuUI.SetActive(true);
        _pauseMenuUI.SetActive(false);

        //Deselecting the options button to avoid it being selected when the options menu is opened
        _optionsButton.GetComponent<Button>().OnDeselect(null);
    }

    //Calls the quit function in the GameplayManager
    public void QuitMenu() => GameplayManager.OnQuit();
}
