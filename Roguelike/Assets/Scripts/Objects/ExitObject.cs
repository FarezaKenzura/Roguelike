using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitObject : CellObject
{
    [SerializeField] private Tile _endTile;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        SingletonHub.Instance.Get<BoardManager>().SetCellTile(coord, _endTile);
    }

    public override void PlayerEntered()
    {
        SingletonHub.Instance.Get<GameManager>().NewLevel();
    }
}
