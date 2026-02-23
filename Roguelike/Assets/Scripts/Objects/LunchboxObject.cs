using UnityEngine;

public class LunchboxObject : CellObject
{
    [SerializeField] private int _buffDuration = 15;

    public override void PlayerEntered()
    {
        Destroy(gameObject);
        SingletonHub.Instance.Get<GameManager>().ActivateLunchbox(_buffDuration);
    }
}
