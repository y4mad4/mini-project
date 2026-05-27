public class EnemyUnit : BattleUnit
{
      public EnemyUnit(EnemyData data) : base(
        data.Id,
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