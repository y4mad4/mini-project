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
    public float Defense;
    public float Speed;
    public bool IsAlive => CurrentHp > 0;

    public BattleUnit(string id, string name, float hp, float mp, float attack, float defense, float speed)
    {
        Id = id;
        Name = name;
        MaxHp = hp;
        CurrentHp = hp;
        MaxMp = mp;
        CurrentMp = mp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
    }

    public float TakeDamage(float damage)
    {
        Debug.Log($"{Name} Defense: {Defense}");
        float final = Mathf.Max(0, damage - Defense);
        CurrentHp = Mathf.Clamp(CurrentHp - final, 0, MaxHp);
        Debug.Log($"{Name} 이 {final} 데미지를 받았습니다. 남은 HP: {CurrentHp}");
        return final;
    }
    public void Heal(float amount)
    {
        CurrentHp = Mathf.Clamp(CurrentHp + amount, 0, MaxHp);
        Debug.Log($"{Name} 이 {amount} 회복했습니다. 남은 HP: {CurrentHp}");
    }

    public void UseMp(float amount)
    {
        CurrentMp = Mathf.Clamp(CurrentMp - amount, 0, MaxMp);
    }
}