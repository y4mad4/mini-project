using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;       // 카메라가 따라갈 대상 (강아지 플레이어)
    public Vector3 offset = new Vector3(0f, 0f, -10f); // 카메라와 플레이어 간의 거리 (Z축 중요!)

    void LateUpdate()
    {
        // 플레이어가 배정되지 않았다면 실행하지 않음
        if (target == null) return;


        transform.position = target.position + offset;
    }
}
