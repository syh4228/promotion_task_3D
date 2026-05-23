using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 2f; // 이동 속도

    [Header("점프 및 물리")]
    [SerializeField] private float _jumpForce = 5f; // 점프 힘
    [SerializeField] private bool _isGrounded; // 지면 체크

    [Header("컴포넌트")]
    [SerializeField] private Rigidbody Rigidbody_Player; // 플레이어 리지드바디 연결
    [SerializeField] private Groundcheck GroundDetecter; // 지면체크 연결
    [SerializeField] private Animatorcontroller Animatorcontroller; // 애니메이션 컨트롤러

    private bool _isDead = false; // 캐릭터 사망 체크
    private bool _isRun = false; // 걷기 체크

    private void OnEnable()
    {
        // 만약 지면 체크 오브젝트가 있으면
        if (GroundDetecter != null)
        {
            // 지면 체크 트리거 이벤트 구독
            GroundDetecter.GroundTriggeredEvent += OnGroundTriggered;
        }
    }

    private void OnDisable()
    {
        if (GroundDetecter != null)
        {
            // 지면 체크 트리거 이벤트 구독 혜지
            GroundDetecter.GroundTriggeredEvent -= OnGroundTriggered;
        }
    }

    private void Start()
    {
        // 게임 시작 시 마우스 커서를 화면 중앙에 고정하고 숨김
        Cursor.lockState = CursorLockMode.Locked;

        if (Animatorcontroller == null)
        {
            Debug.LogError("플레이어 애니메이터 연결 확인 요망!");
        }

        if (Rigidbody_Player == null)
        {
            Rigidbody_Player = GetComponent<Rigidbody>();

            if (Rigidbody_Player == null)
            {
                Debug.LogError("플레이어 리지드바디 연결 확인 요망!");
            }
        }

        if (GroundDetecter == null)
        {
            Debug.LogWarning("플레이어 지면체크 오브젝트 연결 확인 요망");
        }
    }

    private void Update()
    {
        if (_isDead) // 만약 죽었다면
        {
            return; // 반환
        }

        // 만약 쉬프트 누르면
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isRun = true; // 트루 처리
        }
        else // 안누르면
        {
            _isRun = false; // 거짓 처리
        }

        MoveOnUpdate(); // 움직임 함수 호출

        // 만약 스페이스바 누르면
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded) // 만약 지면에 붙어 있으면
            {
                StartJump(); // 점프 함수 호출
            }
        }

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭하면
        {
            StartAttack(); // 공격 함수 호출
        }
    }

    void MoveOnUpdate() // 움직임 함수
    {
        // A(-1),D(1) 입력을 받아 좌우 움직임
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 캐릭터가 바라보는 방향 저장
        Vector3 moveDirection = (transform.right * x) + (transform.forward * z);

        // 대각선 이동 속도 정규화
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        if (moveDirection.magnitude == 0) // 입력이 전혀 없으면 대기
        {
            Animatorcontroller.SetState(AllState.Idle); // 대기 애니메이션 실행

            // 플레이어 움직이지 못하게 속도 0
            Rigidbody_Player.linearVelocity = new Vector3(0, Rigidbody_Player.linearVelocity.y, 0);
        }
        else // 입력이 있으면
        {
            if (_isRun == true) // 달리기 상태면
            {
                // 달리기 애니메이션 실행
                Animatorcontroller.SetState(AllState.Run);
                // 달리기 속도 저장
                float runSpeed = _moveSpeed * 2f;

                // 달리기 속도 계산
                Rigidbody_Player.linearVelocity = new Vector3(moveDirection.x * runSpeed, Rigidbody_Player.linearVelocity.y, moveDirection.z * runSpeed);
            }
            else // 걷기 상태면
            {
                // 걷기 애니메이션 실행
                Animatorcontroller.SetState(AllState.Walk);

                // 걷기 속도 계산
                Rigidbody_Player.linearVelocity = new Vector3(moveDirection.x * _moveSpeed, Rigidbody_Player.linearVelocity.y, moveDirection.z * _moveSpeed);
            }
        }
    }

    void StartJump() // 점프 함수 
    {
        // 점프 힘 계산
        Rigidbody_Player.linearVelocity = new Vector3(Rigidbody_Player.linearVelocity.x, _jumpForce, Rigidbody_Player.linearVelocity.z);
        _isGrounded = false; // 지면체크 false
    }

    void StartAttack() // 공격함수
    {
        // 공격 애니메이션 호출
        Animatorcontroller.SetState(AllState.Attack);
    }

    private void OnGroundTriggered(bool isGrounded) // 지면 체크 센서 트리거 함수
    {
        // 지면 체크 결과 저장
        _isGrounded = isGrounded;
    }
}
