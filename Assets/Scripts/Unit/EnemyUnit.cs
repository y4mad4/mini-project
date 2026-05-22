public class EnemyUnit : BattleUnit
{
    public string Id;

    public EnemyUnit(EnemyData data) : base(
        data.Name,
        data.Hp,
        0,
        data.Attack,
        data.Defense,
        data.Speed)
    {
        Id = data.Id;
    }
}