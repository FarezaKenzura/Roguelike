using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private Label _foodLabel;
    private Label _gameOverMessage;
    private VisualElement _gameOverPanel;

    private void Awake()
    {
        SingletonHub.Instance.Register(this);
    }

    private void OnEnable()
    {
        _foodLabel = _uiDocument.rootVisualElement.Q<Label>("FoodLabel");
        _gameOverPanel = _uiDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");
    }

    public void UpdateFood(int amount)
    {
        _foodLabel.text = "Food: " + amount;
    }

    public void ShowGameOver(int days)
    {
        _gameOverPanel.style.visibility = Visibility.Visible;
        _gameOverMessage.text = "Game Over!\n\nSurvived " + days + " days";
    }

    public void HideGameOver()
    {
        _gameOverPanel.style.visibility = Visibility.Hidden;
    }
}
