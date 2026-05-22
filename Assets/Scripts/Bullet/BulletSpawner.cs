using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private RectTransform battleBox;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float bulletSpeed = 150f;
    [SerializeField] private RectTransform playerHeart;
    [SerializeField] private PlayerStats playerStats;

    private float _timer;

    private bool _isSpawning;

    public void StartSpawning()
    {
        _isSpawning = true;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    public void SetData(float speed, float interval)
    {
        bulletSpeed = speed;
        spawnInterval = interval;
    }

    private void Update()
    {

        if (!_isSpawning) return;
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnBullet();
        }
    }

    private void SpawnBullet()
    {
        // 박스 네 방향 중 랜덤으로 시작 위치 결정
        float halfW = battleBox.rect.width / 2f;
        float halfH = battleBox.rect.height / 2f;

        int side = Random.Range(0, 4); // 0=위 1=아래 2=왼 3=오른
        Vector2 spawnPos;
        Vector2 direction;

        switch (side)
        {
            case 0: // 위에서 아래로
                spawnPos = new Vector2(Random.Range(-halfW, halfW), halfH);
                direction = Vector2.down;
                break;
            case 1: // 아래에서 위로
                spawnPos = new Vector2(Random.Range(-halfW, halfW), -halfH);
                direction = Vector2.up;
                break;
            case 2: // 왼쪽에서 오른쪽으로
                spawnPos = new Vector2(-halfW, Random.Range(-halfH, halfH));
                direction = Vector2.right;
                break;
            default: // 오른쪽에서 왼쪽으로
                spawnPos = new Vector2(halfW, Random.Range(-halfH, halfH));
                direction = Vector2.left;
                break;
        }

        GameObject bullet = Instantiate(bulletPrefab, battleBox);
        RectTransform rt = bullet.GetComponent<RectTransform>();
        rt.anchoredPosition = spawnPos;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Init(direction, bulletSpeed, playerHeart, playerStats, battleBox);
    }
}