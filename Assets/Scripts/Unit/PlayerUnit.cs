public class PlayerUnit : BattleUnit
{
    public int SkillId;

    public PlayerUnit(PlayerData data) : base(
        data.Name,
        data.Hp,
        data.Mp,
        data.Attack,
        data.Defense,
        data.Speed)
    {
    }
}