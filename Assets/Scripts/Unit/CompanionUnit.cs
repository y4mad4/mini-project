public class CompanionUnit : BattleUnit
{
    public string SkillId;

    public CompanionUnit(CompanionData data) : base(
        data.Name,
        data.Hp,
        data.Mp,
        data.Attack,
        data.Defense,
        data.Speed)
    {
        SkillId = data.SkillId;
    }
}