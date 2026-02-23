using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public event System.Action OnTick;
    private int _turnCount;

    private void Awake()
    {
        SingletonHub.Instance.Register(this);
    }

    public void Tick()
    {
        _turnCount += 1;
        OnTick?.Invoke();
        Debug.Log("Current turn count : " + _turnCount);
    }
}
