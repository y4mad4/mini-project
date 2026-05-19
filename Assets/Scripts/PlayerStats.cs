using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject battleCanvas;
    [SerializeField] private GameObject gameOverUI;

    private float _currentHp;

    private void Awake()
    {
        _currentHp = maxHp;
        hpBar.value = _currentHp;
        hpBar.maxValue = maxHp;
        gameOverUI.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0f, maxHp);
        hpBar.value = _currentHp;

        if (_currentHp <= 0f)
            GameOver();
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}