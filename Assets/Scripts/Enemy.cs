using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private float timeForDamage = 20f;
    [SerializeField] private Slider hpBar;

    private float _currentHp;
    private bool _isPlayerInRange;
    private bool _isInBattle;
    private GameObject _battleCanvas;
    float _damageTimer;

    private void Start()
    {
        _currentHp = maxHp;
        _battleCanvas = GameObject.Find("BattleCanvas");
        if (_battleCanvas != null)
            _battleCanvas.SetActive(false);
    }

    private void Awake()
    {
        _currentHp = maxHp;
        hpBar.value = _currentHp;
        hpBar.maxValue = maxHp;
       
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


        _damageTimer += Time.deltaTime;

        if (_isInBattle)
        {
            _damageTimer += Time.deltaTime;

            if (_damageTimer >= 1)
            {
                _damageTimer -= 1;
                TakeDamage(timeForDamage);
            }
        }

    }

    private void StartBattle()
    {
        _isInBattle = true;
        _battleCanvas.SetActive(true);
        bulletSpawner.StartSpawning();
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0f, maxHp);

        hpBar.value = _currentHp;

        Debug.Log("적 HP: " + _currentHp);

        if (_currentHp <= 0f)
            EndBattle();
    }

    private void EndBattle()
    {
        _isInBattle = false;
        _battleCanvas.SetActive(false);
        bulletSpawner.StopSpawning();
        Destroy(gameObject);
    }
}