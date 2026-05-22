using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f; // 위로 올라가는 속도
    [SerializeField] private float alphaSpeed = 2f; // 투명해지는 속도
    [SerializeField] private float destroyTime = 1f; // 몇 초 뒤에 삭제될지

    private TextMeshProUGUI _textMesh;
    private Color _textColor;

    public void Setup(float damage)
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMesh.text = damage.ToString(); // 넘겨받은 데미지 숫자로 텍스트 변경
        _textColor = _textMesh.color;

        Destroy(gameObject, destroyTime); // 정해진 시간 뒤에 이 오브젝트 파괴
    }

    private void Update()
    {
        // 1. 위로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 2. 서서히 투명해지기
        _textColor.a -= alphaSpeed * Time.deltaTime;
        _textMesh.color = _textColor;
    }
}