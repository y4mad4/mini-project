using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHeart : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private RectTransform battleBox;

    private RectTransform _rectTransform;
    private Vector2 _inputVector;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void SetInput(Vector2 input)
    {
        _inputVector = input;
    }

    public void OnMove(InputValue value)
    {
        _inputVector = value.Get<Vector2>();
    }



    private void Update()
    {
        // 이동
        Vector2 newPos = _rectTransform.anchoredPosition;
        newPos += _inputVector * moveSpeed * Time.deltaTime;

        // 박스 범위 제한
        float halfBoxWidth = battleBox.rect.width / 2f;
        float halfBoxHeight = battleBox.rect.height / 2f;
        float halfHeartSize = _rectTransform.rect.width / 2f;

        newPos.x = Mathf.Clamp(newPos.x, -halfBoxWidth + halfHeartSize, halfBoxWidth - halfHeartSize);
        newPos.y = Mathf.Clamp(newPos.y, -halfBoxHeight + halfHeartSize, halfBoxHeight - halfHeartSize);

        _rectTransform.anchoredPosition = newPos;
    }
}
