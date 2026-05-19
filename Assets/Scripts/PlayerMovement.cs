using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerHeart _playerHeart;

    private Rigidbody _rb;
    private Vector2 _inputVector;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    public void OnMove(InputValue value)
    {
        _inputVector = value.Get<Vector2>();

        // 하트에게 입력값 전달
        if (_playerHeart != null)
            _playerHeart.SetInput(_inputVector);
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // 전투 중이면 이동 막기
        if (GameObject.Find("BattleCanvas") != null &&
            GameObject.Find("BattleCanvas").activeSelf)
            return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // 4방향 제한
        Vector2 input = _inputVector;
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            input.y = 0f;
        else
            input.x = 0f;

        // 실제 이동
        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;
        Vector3 targetVelocity = moveDir * moveSpeed;
        targetVelocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = targetVelocity;
    }
}