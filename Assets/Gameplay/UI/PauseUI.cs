using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    [SerializeField, Tooltip("The pause menu.")]
    private GameObject _pauseMenuUI;

    private PlayerControls _playerActions;

    private InputAction _menu;

    public UnityEvent OnPause;
    public UnityEvent OnUnPause;

    private void Awake()
    {
        //Creating Player Controller
        _playerActions = new PlayerControls();
        _pauseMenuUI.SetActive(false);
    }

    //private void OnEnable()
    //{
    //    //Storing and enabling pause action
    //    _menu = _playerActions.Menu.Pause;
    //    _menu.Enable();
        
    //    _menu.performed += MenuCall;
    //}

    //private void OnDisable() => _menu.Disable();

    public void MenuCall(InputAction.CallbackContext context)
    {
        //Only running once while button is pressed
        if (!context.started)
            return;

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
        
        GameplayManager.OnPause();

        OnPause.Invoke();
    }

    //Unpauses the game
    public void Resume()
    {
        //Deactivating pause menu
        _pauseMenuUI.SetActive(false);

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

    }

    //Loads the main menu scene
    public void QuitMenu()
    {
        GameplayManager.OnQuit();

        OnUnPause.Invoke();
    }
}
