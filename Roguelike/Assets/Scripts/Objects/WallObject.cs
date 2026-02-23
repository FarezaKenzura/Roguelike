using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    [SerializeField] private int _maxHealth = 3;

    [SerializeField] private Tile _obstacleTile;
    [SerializeField] private Tile _destroyedTile;

    private int _healthPoint;
    private Tile _originalTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        _healthPoint = _maxHealth;

        _originalTile = SingletonHub.Instance.Get<BoardManager>().GetCellTile(cell);
        SingletonHub.Instance.Get<BoardManager>().SetCellTile(cell, _obstacleTile);
    }

    public override bool PlayerWantsToEnter()
    {
        _healthPoint -= 1;
        if (_healthPoint == 1)
        {
            SingletonHub.Instance.Get<BoardManager>().SetCellTile(m_Cell, _destroyedTile);
            return false;
        }

        if (_healthPoint <= 0)
        {
            SingletonHub.Instance.Get<BoardManager>().SetCellTile(m_Cell, _originalTile);
            Destroy(gameObject);
            return false;
        }

        return false;
    }
}
