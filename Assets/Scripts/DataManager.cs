using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private Dictionary<string, PlayerData> _playerDict = new Dictionary<string, PlayerData>();
    private Dictionary<string, CompanionData> _companionDict = new Dictionary<string, CompanionData>();
    private Dictionary<string, SkillData> _skillDict = new Dictionary<string, SkillData>();
    private Dictionary<string, ItemData> _itemDict = new Dictionary<string, ItemData>();
    private Dictionary<string, EnemyData> _enemyDict = new Dictionary<string, EnemyData>();
    private Dictionary<string, BulletData> _bulletDict = new Dictionary<string, BulletData>();
    private Dictionary<string, WeaponData> _weaponDict = new Dictionary<string, WeaponData>();
    private Dictionary<string, ArmorData> _armorDict = new Dictionary<string, ArmorData>();
    private Dictionary<string, AccessoryData> _accessoryDict = new Dictionary<string, AccessoryData>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadAll();
    }

    private void LoadAll()
    {
        _playerDict = Load<PlayerData>("Player");
        _companionDict = Load<CompanionData>("Companion");
        _skillDict = Load<SkillData>("Skill");
        _itemDict = Load<ItemData>("Item");
        _enemyDict = Load<EnemyData>("Enemy");
        _bulletDict = Load<BulletData>("BulletData");
        _weaponDict = Load<WeaponData>("Weapon");
        _armorDict = Load<ArmorData>("Armor");
        _accessoryDict = Load<AccessoryData>("Accessory");

        Debug.Log("데이터 로드 완료");
    }

    private Dictionary<string, T> Load<T>(string fileName) where T : class
    {
        TextAsset json = Resources.Load<TextAsset>(fileName);
        List<T> list = JsonConvert.DeserializeObject<List<T>>(json.text);
        Dictionary<string, T> dict = new Dictionary<string, T>();

        foreach (T item in list)
        {
            string id = item.GetType().GetField("Id").GetValue(item).ToString();
            dict[id] = item;
        }

        Debug.Log($"{fileName} 로드 완료: {dict.Count}개");
        return dict;
    }

    public PlayerData GetPlayer(string id) => GetData(_playerDict, id);
    public CompanionData GetCompanion(string id) => GetData(_companionDict, id);
    public SkillData GetSkill(string id) => GetData(_skillDict, id);
    public ItemData GetItem(string id) => GetData(_itemDict, id);
    public EnemyData GetEnemy(string id) => GetData(_enemyDict, id);
    public BulletData GetBullet(string id) => GetData(_bulletDict, id);
    public WeaponData GetWeapon(string id) => GetData(_weaponDict, id);
    public ArmorData GetArmor(string id) => GetData(_armorDict, id);
    public AccessoryData GetAccessory(string id) => GetData(_accessoryDict, id);

    private T GetData<T>(Dictionary<string, T> dict, string id)
    {
        if (dict.TryGetValue(id, out T data))
            return data;

        Debug.LogWarning($"데이터 없음: {typeof(T).Name} id={id}");
        return default;
    }
}