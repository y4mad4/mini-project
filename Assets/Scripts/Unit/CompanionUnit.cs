public class CompanionUnit : BattleUnit
{
    public string Skill1Id;
    public string Skill2Id;
    public string Skill3Id;

    public CompanionUnit(CompanionData data) : base(
        data.Id,
        data.Name,
        data.Hp,
        data.Mp,
        data.Attack,
        data.MagicAttack,
        data.Defense,
        data.MagicDefense,
        data.Speed)
    {
        Skill1Id = data.Skill1Id;
        Skill2Id = data.Skill2Id;
        Skill3Id = data.Skill3Id;
    }
}