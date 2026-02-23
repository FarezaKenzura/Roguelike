using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    public System.Func<Vector2Int> GetTargetPosition;

    [Header("Map Components")]
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _tilemap;

    [Header("Board Settings")]
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField, Range(1, 5)] private int _foodCount;

    [Header("Entity Prefabs")]
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private ExitObject _exitPrefab;
    [SerializeField] private WallObject[] _wallPrefab;
    [SerializeField] private FoodObject[] _foodPrefab;
    [SerializeField] private LunchboxObject[] _lunchboxPrefab;

    [Header("Tiles Data")]
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _wallTiles;

    private CellData[,] _boardData;
    private List<Vector2Int> _emptyCellsList;

    private void Awake()
    {
        SingletonHub.Instance.Register(this);
    }

    public void Init(int level)
    {
        _width = 8 + level;
        _height = 8 + level;

        _emptyCellsList = new List<Vector2Int>();
        _boardData = new CellData[_width, _height];

        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                Tile tile;
                _boardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == _width - 1 || y == _height - 1)
                {
                    tile = _wallTiles[Random.Range(0, _wallTiles.Length)];
                    _boardData[x, y].Passable = false;
                }
                else
                {
                    tile = _groundTiles[Random.Range(0, _groundTiles.Length)];
                    _boardData[x, y].Passable = true;

                    _emptyCellsList.Add(new Vector2Int(x, y));
                }

                _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        _emptyCellsList.Remove(new Vector2Int(1, 1));

        GeneratePlayer();
        GenerateEnemy(level);
        GenerateExit();
        GenerateWall();
        GenerateFood();
        GenerateLunchbox();
    }

    public void Clean()
    {
        if (_boardData == null) return;

        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                var cellData = _boardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

    public void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = _boardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    public void GeneratePlayer()
    {
        Vector2Int startCoord = new Vector2Int(1, 1);
        Player newPlayer = Instantiate(_playerPrefab);
        AddObject(newPlayer, startCoord);
        newPlayer.MoveTo(startCoord, true);
        GetTargetPosition = () => newPlayer.Cell;
    }

    public void GenerateEnemy(int level)
    {
        int enemyCount = level / 2 + 1;

        for (int i = 0; i < enemyCount; i++)
        {
            int randomCell = Random.Range(0, _emptyCellsList.Count);
            Vector2Int coord = _emptyCellsList[randomCell];
            _emptyCellsList.RemoveAt(randomCell);

            Enemy newEnemy = Instantiate(_enemyPrefab);
            AddObject(newEnemy, coord);
        }
    }

    public void GenerateFood()
    {
        for (int i = 0; i < _foodCount; ++i)
        {
            int randomCell = Random.Range(0, _emptyCellsList.Count);
            int randomFood = Random.Range(0, _foodPrefab.Length);
            Vector2Int coord = _emptyCellsList[randomCell];

            _emptyCellsList.RemoveAt(randomCell);
            FoodObject newFood = Instantiate(_foodPrefab[randomFood]);
            AddObject(newFood, coord);
        }
    }

    public void GenerateLunchbox()
    {
        int lunchboxCount = Random.Range(0, 2);
        for (int i = 0; i < lunchboxCount; ++i)
        {
            int randomCell = Random.Range(0, _emptyCellsList.Count);
            int randomLunchbox = Random.Range(0, _lunchboxPrefab.Length);
            Vector2Int coord = _emptyCellsList[randomCell];

            _emptyCellsList.RemoveAt(randomCell);
            LunchboxObject newLunchbox = Instantiate(_lunchboxPrefab[randomLunchbox]);
            AddObject(newLunchbox, coord);
        }
    }

    public void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomCell = Random.Range(0, _emptyCellsList.Count);
            int randomWall = Random.Range(0, _wallPrefab.Length);
            Vector2Int coord = _emptyCellsList[randomCell];

            _emptyCellsList.RemoveAt(randomCell);
            WallObject newWall = Instantiate(_wallPrefab[randomWall]);
            AddObject(newWall, coord);
        }
    }

    public void GenerateExit()
    {
        Vector2Int endCoord = new Vector2Int(_width - 2, _height - 2);
        AddObject(Instantiate(_exitPrefab), endCoord);
        _emptyCellsList.Remove(endCoord);
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        _tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= _width || cellIndex.y < 0 || cellIndex.y >= _height)
        {
            return null;
        }
        return _boardData[cellIndex.x, cellIndex.y];
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return _tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }
}
