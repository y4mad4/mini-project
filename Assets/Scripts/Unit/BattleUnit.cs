using UnityEngine;

public class BattleUnit
{
    public string Id;
    public string Name;
    public float MaxHp;
    public float CurrentHp;
    public float MaxMp;
    public float CurrentMp;
    public float Attack;
    public float MagicAttack;
    public float Defense;
    public float MagicDefense;
    public float Speed;
    public bool IsAlive => CurrentHp > 0;

    public BattleUnit(string id, string name, float hp, float mp, float attack, float magicAttack, float defense, float magicDefense, float speed)
    {
        Id = id;
        Name = name;
        MaxHp = hp;
        CurrentHp = hp;
        MaxMp = mp;
        CurrentMp = mp;
        Attack = attack;
        MagicAttack = magicAttack;
        Defense = defense;
        MagicDefense = magicDefense;
        Speed = speed;
    }

    public float TakeDamage(float damage, bool isMagic = false)
    {
        float def = isMagic ? MagicDefense : Defense;
        float final = Mathf.Max(1, damage - def);
        CurrentHp = Mathf.Clamp(CurrentHp - final, 0, MaxHp);
        return final;
    }

    public void Heal(float amount)
    {
        CurrentHp = Mathf.Clamp(CurrentHp + amount, 0, MaxHp);
    }

    public void UseMp(float amount)
    {
        CurrentMp = Mathf.Clamp(CurrentMp - amount, 0, MaxMp);
    }
}