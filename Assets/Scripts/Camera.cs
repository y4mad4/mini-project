using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적할 대상")]
    public Transform target;

    [Header("카메라 오프셋 (거리)")]
    // X는 무조건 0, Y와 Z만 앵글을 위해 10, -10으로 고정합니다.
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    private void LateUpdate()
    {
        if (target == null) return;

        // 중간 계산(Lerp) 없이 플레이어 위치 + 오프셋 좌표로 카메라를 매 프레임 즉시 순간이동 시킵니다.
        transform.position = target.position + offset;
    }
}