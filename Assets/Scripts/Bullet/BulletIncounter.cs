using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BulletIncounter : MonoBehaviour
{
    [SerializeField] private int enemyId = 1; // 인스펙터에서 적 id 지정
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Slider EnemyHpBar;

    private BulletData _data;
    private float _currentHp;
    private bool _isPlayerInRange;
    private bool _isInBattle;
    private float _damageTimer;
    private GameObject _battleCanvas;



    private void Start()
    {
        // 데이터 로드
        _data = BulletDataManager.Instance.GetBulletData(enemyId);
        _currentHp = _data.maxHp;
        EnemyHpBar.maxValue = _data.maxHp;
        EnemyHpBar.value = _currentHp;
        // 탄막 속도, 간격 적용
        bulletSpawner.SetData(_data.bulletSpeed, _data.bulletSpawnInterval);

        _battleCanvas = GameObject.Find("BulletBattleCanvas");
        if (_battleCanvas != null)
            _battleCanvas.SetActive(false);

         
}



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _isPlayerInRange = false;
    }

    private void Update()
    {
        if (_isPlayerInRange && !_isInBattle && Keyboard.current.zKey.wasPressedThisFrame)
            StartBattle();

        if (_isInBattle)
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= 1f)
            {
                _damageTimer -= 1f;
   
                TakeDamage(_data.damagePerSecond);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0f, _data.maxHp);
        EnemyHpBar.value = _currentHp;

        Debug.Log("적 HP: " + _currentHp);

        if (_currentHp <= 0f)
            EndBattle();
    }
    private void StartBattle()
    {
        _isInBattle = true;
        _battleCanvas.SetActive(true);
        bulletSpawner.StartSpawning();
    }

    private void EndBattle()
    {
        _isInBattle = false;
        _battleCanvas.SetActive(false);
        bulletSpawner.StopSpawning();
        Destroy(gameObject);
    }
}