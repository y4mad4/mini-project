using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BulletDataManager : MonoBehaviour
{
    public static BulletDataManager Instance;

    private Dictionary<int, BulletData> _BulletDict = new Dictionary<int, BulletData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadData();
    }

    private void LoadData()
    {
        TextAsset json = Resources.Load<TextAsset>("BulletData");
        List<BulletData> list = JsonConvert.DeserializeObject<List<BulletData>>(json.text);

        foreach (BulletData data in list)
            _BulletDict[data.Id] = data;

        Debug.Log("적 데이터 로드 완료: " + _BulletDict.Count + "개");
    }

    public BulletData GetBulletData(int id)
    {
        if (_BulletDict.TryGetValue(id, out BulletData data))
            return data;

        Debug.LogWarning("적 데이터 없음: " + id);
        return null;
    }
}