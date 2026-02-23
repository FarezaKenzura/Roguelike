using Unity.Mathematics;
using UnityEngine;

public class Enemy : CellObject
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _health = 3;
    [SerializeField] private float _moveSpeed = 5.0f;

    private int _currentHealth;
    private bool _isMoving;
    private Vector3 _moveTarget;

    private void Awake()
    {
        SingletonHub.Instance.Get<TurnManager>().OnTick += TurnHappen;
    }

    private void OnDestroy()
    {
        SingletonHub.Instance.Get<TurnManager>().OnTick -= TurnHappen;
    }

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);

            if (transform.position == _moveTarget)
            {
                _isMoving = false;
                _animator.SetBool("Moving", false);
            }
        }
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        _currentHealth = _health;
    }

    public override bool PlayerWantsToEnter()
    {
        _currentHealth -= 1;
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        return false;
    }

    private bool MoveTo(Vector2Int coord)
    {
        var board = SingletonHub.Instance.Get<BoardManager>();
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        board.GetCellData(m_Cell).ContainedObject = null;
        targetCell.ContainedObject = this;
        m_Cell = coord;

        _moveTarget = board.CellToWorld(coord);
        _isMoving = true;

        _animator.SetBool("Moving", true);

        return true;
    }

    private void TurnHappen()
    {
        var playerCell = SingletonHub.Instance.Get<BoardManager>().GetTargetPosition.Invoke();

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = math.abs(xDist);
        int absYDist = math.abs(yDist);

        if ((math.abs(xDist) + math.abs(yDist)) == 1)
        {
            _animator.SetTrigger("Attack");
            SingletonHub.Instance.Get<GameManager>().ChangeFood(-3);
        }
        else
        {
            if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
            {
                SingletonHub.Instance.Get<GameManager>().ChangeFood(3);
            }
            else
            {
                if (absXDist > absYDist)
                {
                    if (!TryMoveInX(xDist))
                    {
                        TryMoveInY(yDist);
                    }
                }
                else
                {
                    if (!TryMoveInY(yDist))
                    {
                        TryMoveInX(xDist);
                    }
                }
            }
        }
    }

    private bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);
    }

    private bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);
    }

    public void DrainPlayerEnergy(CellObject target)
    {
        IEnergyLoss receiver = target as IEnergyLoss;
        if (receiver != null)
        {
            int drainPower = 10;
            receiver.OnEnergyDrained(drainPower);
        }
    }
}
