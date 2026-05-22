using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 _direction;
    private float _speed;
    private RectTransform _rectTransform;
    private RectTransform _heartRect;
    private PlayerStats _playerStats;
    private float _damage = 10f;
    private RectTransform _battleBox;

    public void Init(Vector2 direction, float speed, RectTransform heartRect, PlayerStats playerStats, RectTransform battleBox)
    {
        _direction = direction;
        _speed = speed;
        _rectTransform = GetComponent<RectTransform>();
        _heartRect = heartRect;
        _playerStats = playerStats;
        _battleBox = battleBox;
    }

    private void Update()
    {
        _rectTransform.anchoredPosition += _direction * _speed * Time.deltaTime;

        // 하트랑 거리 계산으로 충돌 감지
        float dist = Vector2.Distance(_rectTransform.anchoredPosition, _heartRect.anchoredPosition);
        if (dist < 15f)
        {
            _playerStats.TakeDamage(_damage);
            Destroy(gameObject);
            return;
        }

        // 박스 밖으로 나가면 삭제
        float halfW = _battleBox.rect.width / 2f;
        float halfH = _battleBox.rect.height / 2f;
        Vector2 pos = _rectTransform.anchoredPosition;

        if (pos.x < -halfW || pos.x > halfW || pos.y < -halfH || pos.y > halfH)
            Destroy(gameObject);
    }
}