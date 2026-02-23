using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _foodAmount = 20;
    private int _currentLevel = 1;
    private int _lunchboxDuration = 0;
    private bool _isGameOver = false;

    public bool IsGameOver => _isGameOver;
    public bool IsBuffActive => _lunchboxDuration > 0;

    private void Awake()
    {
        SingletonHub.Instance.Register(this);
    }

    private void Start()
    {
        SingletonHub.Instance.Get<TurnManager>().OnTick += OnTurnHappen;
        StartNewGame();
    }

    public void StartNewGame()
    {
        _isGameOver = false;
        _currentLevel = 1;
        _foodAmount = 20;
        _lunchboxDuration = 0;

        SingletonHub.Instance.Get<UIManager>().HideGameOver();
        SingletonHub.Instance.Get<UIManager>().UpdateFood(_foodAmount);

        RefreshBoard();
    }

    public void NewLevel()
    {
        _currentLevel++;
        RefreshBoard();
    }

    private void RefreshBoard()
    {
        SingletonHub.Instance.Get<BoardManager>().Clean();
        SingletonHub.Instance.Get<BoardManager>().Init(_currentLevel);
    }

    private void OnTurnHappen()
    {
        if (_isGameOver) return;

        if (_lunchboxDuration > 0)
        {
            _lunchboxDuration--;
            if (_lunchboxDuration <= 0) Debug.Log("Buff Lunchbox Habis!");
        }

        ChangeFood(-1);
    }

    public void ActivateLunchbox(int duration)
    {
        _lunchboxDuration = duration;
    }

    public void ChangeFood(int amount)
    {
        if (_isGameOver) return;

        if (amount > 0 && IsBuffActive)
        {
            amount *= 2;
            Debug.Log("Food di-double oleh Buff Lunchbox!");
        }

        _foodAmount += amount;
        SingletonHub.Instance.Get<UIManager>().UpdateFood(_foodAmount);

        if (_foodAmount <= 0)
        {
            _isGameOver = true;
            SingletonHub.Instance.Get<UIManager>().ShowGameOver(_currentLevel);

        }
    }
}
