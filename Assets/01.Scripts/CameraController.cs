using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 대상")]
    [SerializeField] private Transform _target; // 카메라가 따라 다닐 대상

    [Header("카메라 설정")]
    [SerializeField] private float _mouseSensitivity = 200f; // 마우스 감도 설정
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -4f); // 카메라와 플레이어 거리

    private float _rotationX = 0f; // 카메라 상하 회전 값 저장 변수

    void Start()
    {
        // 마우스 커서 화면에서 숨기고 중앙 고정
        Cursor.lockState = CursorLockMode.Locked;

        if (_target == null) // 타겟 없으면
        {
            Debug.LogWarning("카메라 추적할 대상 연결 확인 요망!");
        }
    }

    void LateUpdate()
    {
        if (_target == null) // 타겟이 없으면
        {
            return;
        }

        RotateCamera(); // 카메라 회전 함수 호출
        FollowTarget(); // 키메라 위치 함수 호출
    }

    // 카메라 회전 함수
    void RotateCamera()
    {
        // 마우스 x축 입력 저장
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        // 마우스 Y축 입력 저장
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        // 상하 회전
        _rotationX -= mouseY;
        // 너무 뒤로 넘어가지 않게 x축 범위 제한
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        this.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

        // 좌우 회전 (플레이어 몸통 전체를 회전)
        this.transform.Rotate(Vector3.up * mouseX);
    }

    // 카메라 위치 함수
    void FollowTarget()
    {
        // 카메라가 위치한 자리 계산
        Vector3 targetPosition = _target.position + _target.up * _offset.y+ _target.forward * _offset.z + _target.right * _offset.x;

        // 카메라 위치로 이동
        this.transform.position = targetPosition;
    }

    public void SetTarget(Transform newTarget) // 카메라 타겟이 바뀔때 사용 함수
    {
        _target = newTarget;
    }
}
