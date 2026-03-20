using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Button _button;
    private List<Button> _menuButtons = new List<Button>();
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _uiDocument = GetComponent<UIDocument>();

        _button = _uiDocument.rootVisualElement.Q("StartButton") as Button;
        _button.RegisterCallback<ClickEvent>(OnStartClicked);

        _menuButtons = _uiDocument.rootVisualElement.Query<Button>().ToList();

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClicked);
        }
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnStartClicked);

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(OnAllButtonsClicked);
        }
    }

    private void OnStartClicked(ClickEvent evt)
    {
        Debug.Log("Start Button Clicked!");
    }

    private void OnAllButtonsClicked(ClickEvent evt)
    {
        _audioSource.Play();
    }
}