using UnityEngine;

public class FoodObject : CellObject
{
    [SerializeField] private int _amountGranted = 10;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        SingletonHub.Instance.Get<GameManager>().ChangeFood(_amountGranted);
    }
}
