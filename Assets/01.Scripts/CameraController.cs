using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 설정")]
    public Transform Target;
    public float Distance = 5.0f; // 캐릭터와 카메라의 거리
    public float HeightOffset = 1.5f; // 캐릭터의 머리 높이

    [Header("마우스 회전 설정")]
    public float MouseSensitivity = 3.0f; // 마우스 감도
    public float PitchMin = -15.0f; // 카메라가 아래로 내려가는 최대 각도
    public float PitchMax = 60.0f;  // 카메라가 위로 올라가는 최대 각도

    private float _pitch = 30.0f; // 위아래 회전값
    private float _yaw = 0.0f;    // 좌우 회전값

    private void Start()
    {
        // 게임 시작 시 마우스 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (Target == null) return;

        // 마우스가 잠겨있을 때만 회전 값을 갱신
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

            _yaw += mouseX;
            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, PitchMin, PitchMax);
        }

        // 회전값 계산
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

        // 카메라 위치 및 바라보는 방향 계산
        Vector3 targetPosition = Target.position + Vector3.up * HeightOffset;

        // 회전값에 따라 거리만큼 뒤로 이동
        transform.position = targetPosition - (rotation * Vector3.forward * Distance);

        // 캐릭터를 바라보게 함
        transform.LookAt(targetPosition);
    }
}
