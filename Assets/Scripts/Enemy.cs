using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    [SerializeField] private string enemyId = "enemy_01";

    private bool _isPlayerInRange;
    private GameObject _battleCanvas;

 

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
        if (_isPlayerInRange && Keyboard.current.zKey.wasPressedThisFrame)
            BattleManager.Instance.StartBattle(enemyId);
    }
}