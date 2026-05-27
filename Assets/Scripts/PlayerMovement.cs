using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    // 💡 착시를 보정하기 위해 상하 이동 시 속도를 얼마나 뻥튀기할지 정하는 변수 (1.2 ~ 1.4 추천)
    [SerializeField] private float verticalSpeedMultiplier = 1.3f;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerHeart _playerHeart;

    [Header("애니메이션")]
    [SerializeField] private Animator _animator;

    // 💡 매 프레임 캔버스를 찾지 않도록 미리 저장해둘 변수
    private GameObject _battleCanvas;

    private Rigidbody _rb;
    private Vector2 _inputVector;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // 🚨 Find는 무조건 Awake나 Start에서 딱 한 번만 해야 합니다!
        _battleCanvas = GameObject.Find("TurnBattleCanvas");
    }

    public void OnMove(InputValue value)
    {
        _inputVector = value.Get<Vector2>();

        if (_playerHeart != null)
            _playerHeart.SetInput(_inputVector);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // 🚨 FixedUpdate(1초에 50번 실행) 안에서 GameObject.Find를 쓰면 게임이 뚝뚝 끊깁니다.
        // 미리 찾아둔 변수로 체크해야 합니다.
        if (_battleCanvas != null && _battleCanvas.activeSelf)
            return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector2 input = _inputVector;
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            input.y = 0f;
        else
            input.x = 0f;

        // 💡 [추가된 로직] 애니메이터에 값 전달하기
        bool isMoving = (input.magnitude > 0);
        _animator.SetBool("isMoving", isMoving);

        // 움직일 때만(키보드를 누를 때만) 방향을 업데이트합니다.
        // 안 그러면 키보드에서 손을 뗐을 때(0,0) 무조건 아래를 보게 됩니다.
        if (isMoving)
        {
            _animator.SetFloat("InputX", input.x);
            _animator.SetFloat("InputY", input.y);
        }

        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

        // 💡 속도 보정 로직: 상하 이동(Y축 입력) 중이라면 속도에 가중치를 곱해줍니다.
        float currentSpeed = moveSpeed;
        if (Mathf.Abs(input.y) > 0)
        {
            currentSpeed *= verticalSpeedMultiplier;
        }

        // 보정된 속도를 적용합니다.
        Vector3 targetVelocity = moveDir * currentSpeed;
        targetVelocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = targetVelocity;
    }
}