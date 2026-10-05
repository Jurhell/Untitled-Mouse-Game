using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField, Tooltip("Array of health UI elements")]
    private GameObject[] _healthUI;

    private void Awake()
    {
        if (_healthUI.Length <= 0)
            Debug.LogError("Health UI array is empty. Please assign health UI elements in the inspector.");
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthUI[GameplayManager.PlayerHealth].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealth()
    {
        if (_healthUI.Length <= 0)
            return;

        if (GameplayManager.PlayerHealth >= 0)
        {
            //Deactivating the current health UI and activate the next one in accordance with the player's health
            _healthUI[GameplayManager.PlayerHealth].SetActive(true);
            _healthUI[GameplayManager.PlayerHealth + 1].SetActive(false);
        }
    }
}
