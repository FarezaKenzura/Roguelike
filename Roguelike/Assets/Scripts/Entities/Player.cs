using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CellObject, IEnergyLoss
{
    private readonly Dictionary<Key, Vector2Int> _inputMap = new()
    {
        { Key.UpArrow,    Vector2Int.up },
        { Key.DownArrow,  Vector2Int.down },
        { Key.LeftArrow,  Vector2Int.left },
        { Key.RightArrow, Vector2Int.right },
        { Key.Space,      Vector2Int.zero }
    };

    [SerializeField] private int _endurance = 2;
    [SerializeField] private int _vigor = 5;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private Animator _animator;
    private Vector2Int _cellPos;
    private Vector3 _target;

    private bool _moving;

    public Vector2Int Cell => _cellPos;

    private void Update()
    {
        if (SingletonHub.Instance.Get<GameManager>().IsGameOver || _moving)
        {
            Process();
            return;
        }

        foreach (var move in _inputMap)
        {
            if (Keyboard.current[move.Key].wasPressedThisFrame)
            {
                AttemptMove(_cellPos + move.Value);
                break;
            }
        }
    }

    private void Process()
    {
        if (SingletonHub.Instance.Get<GameManager>().IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
                SingletonHub.Instance.Get<GameManager>().StartNewGame();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, _moveSpeed * Time.deltaTime);
            if (transform.position == _target) StopMoving();
        }
    }

    private void AttemptMove(Vector2Int next)
    {
        var cell = SingletonHub.Instance.Get<BoardManager>().GetCellData(next);
        if (cell == null || !cell.Passable) return;

        SingletonHub.Instance.Get<TurnManager>().Tick();

        if (cell.ContainedObject == null || cell.ContainedObject.PlayerWantsToEnter())
            MoveTo(next, false);
        else
            _animator.SetTrigger("Attack");
    }

    private void StopMoving()
    {
        _moving = false;
        _animator.SetBool("Moving", false);
        SingletonHub.Instance.Get<BoardManager>().GetCellData(_cellPos).ContainedObject?.PlayerEntered();
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        _cellPos = cell;
        _target = SingletonHub.Instance.Get<BoardManager>().CellToWorld(cell);
        _moving = !immediate;
        if (immediate) transform.position = _target;
        _animator.SetBool("Moving", _moving);
    }

    public void OnEnergyDrained(int amount)
    {
        int finalDrain = Mathf.Max(1, amount - _endurance);

        SingletonHub.Instance.Get<GameManager>().ChangeFood(-finalDrain);
    }
}
