using UnityEngine;
// 1. 새로운 인풋 시스템을 쓰기 위해 네임스페이스를 추가합니다.
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 2. 새로운 인풋 시스템 방식의 키보드 입력 받기 (WASD / 방향키 기본 대응)
        // 키보드가 연결되어 있다면 vector2 값을 알아서 가져옵니다.
        if (Keyboard.current != null)
        {
            float x = 0;
            float y = 0;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1f;

            moveInput = new Vector2(x, y).normalized;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}