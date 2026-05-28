public class EnemyUnit : BattleUnit
{
    public int ExpReward;
    public string DropItemId;
    public float DropRate;

    public EnemyUnit(EnemyData data) : base(
        data.Id,
        data.Name,
        data.Hp,
        0,
        data.Attack,
        data.MagicAttack,
        data.Defense,
        data.MagicDefense,
        data.Speed)
    {
        ExpReward = data.ExpReward;
        DropItemId = data.DropItemId;
        DropRate = data.DropRate;
    }
}