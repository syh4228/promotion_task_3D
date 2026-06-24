using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour

{
    [Header("이동 설정")]
    public float _moveSpeed = 5.0f; // 이동속도

    [Header("공격 설정")]
    [SerializeField] private float _attackCooldown = 0.5f; // 공격 쿨타임
    [SerializeField] private float _attackRange = 2.0f; // 공격 범위
    private float _cooldownTimer = 0f; // 쿨타임 계산

    [Header("점프 설정")]
    public float JumpForec = 7.0f;

    [Header("줍기 설정")]
    [SerializeField] private float _pickupRange = 2.0f; // 아이템 인식 범위

    [Header("피격 설정")]
    [SerializeField] private float _hitStunDuration = 0.4f; // 플레이어 경직 시간
    private float _stunTimer = 0f; // 경직 타이머

    [Header("컴포넌트")]
    [SerializeField] private Rigidbody _rigidbody; // 리지드바디 연결
    [SerializeField] private Groundcheck _groundCheck;
    [SerializeField] private Animatorcontroller _animatorController;
    [SerializeField] private InventoryManager _inventoryManager; // 인벤토리 연결
    [SerializeField] private PlayerState _playerStat; // 스텟 연결

    private Vector3 _moveDirection;
    private bool _jumpRequested;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("리지드바디가 없습니다!");
        }

        if (_playerStat == null)
        {
            _playerStat = GetComponent<PlayerState>();
        }
    }

    private void Update()
    {
        if (_stunTimer > 0f) // 경직 타이머가 0보다 크면
        {
            _stunTimer = _stunTimer - Time.deltaTime; // 타이머 시간 증가
            _moveDirection = Vector3.zero; // 경직 중 이동 관성 제거
            return; // 반환
        }

        if (_cooldownTimer > 0f) // 만약 쿨타임이 0 보다 크면
        {
            // 쿨타임 시작
            _cooldownTimer = _cooldownTimer - Time.deltaTime;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 카메라의 앞/뒤(z)와 좌/우(x)를 합쳐서 최종 이동 방향 결정
        _moveDirection = (camForward * z + camRight * x);

        if (_moveDirection.magnitude > 0.1f)
        {
            _moveDirection.Normalize();
        }

        bool isGrounded = _groundCheck != null && _groundCheck.IsGrounded;

        if (isGrounded == true)
        {
            _animatorController.SetJump(false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _jumpRequested = true;
            _animatorController.SetJump(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_cooldownTimer <= 0f) // 쿨타임이 0이거나 0보다 작으면
            {
                StartAttack(); // 공격 함수 호출
            }
        }

        // 구르기 (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _animatorController.SetState(AllState.Roll);
        }

        // 상호작용 / 말걸기 (E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            _animatorController.SetState(AllState.Talk);
        }

        // 줍기 (V)
        if (Input.GetKeyDown(KeyCode.V))
        {
            TryPickupItem();
        }
    }

    private void FixedUpdate()
    {
        if (_stunTimer > 0f) // 스턴 타이머 0보다 크면
        {
            // 미끄러짐 방지를 위해 Y축(중력)을 제외한 X, Z 속도를 0으로 만듦
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
            return;
        }

        Move();
        Rotate();

        if (_jumpRequested)
        {
            Jump();
            _jumpRequested = false;
        }
    }

    private void Move()
    {
        float speed; // 속도 변수 저장

        if (_moveDirection.magnitude < 0.1f)
        {
            speed = 0f;
            _animatorController.SetState(AllState.Idle); // 대기 애니메이션 실행
        }
        else
        {
            speed = _moveSpeed; // 속도 적용
            _animatorController.SetState(AllState.Run); // Run 애니메이션
        }

        _rigidbody.MovePosition(_rigidbody.position + _moveDirection * speed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        if (_moveDirection.magnitude > 0.1f)
        {
            // 봐라 보려고 하는 회전 방향
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

            // 현재 방향에서 목표 방향으로 '초당 10f의 속도'로 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void Jump()
    {
        _rigidbody.AddForce(Vector3.up * JumpForec, ForceMode.Impulse);
    }


    void StartAttack() // 공격함수
    {
        // 공격 애니메이션 호출
        _animatorController.SetState(AllState.Attack);
        _cooldownTimer = _attackCooldown; // 쿨타임 초기화

        // 공격 범위안에 콜라이더 전부 배열로 저장
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _attackRange);

        foreach (Collider enemyCollider in hitEnemies) // 하나씩 꺼내서 확인
        {
            // 슬라임 스텟 컴포넌트를 가지고 있으면 저장
            SlimeState slime = enemyCollider.GetComponent<SlimeState>();

            // 슬라임이 있고, 슬라임이 죽지 않았다면
            if (slime != null && slime.IsDead == false)
            {
                // 배틀매니저 인스턴스가 있고, 플레이어 스탯이 있으면
                if (BattleManager.Instance != null && _playerStat != null)
                {
                    // 배틀 매니저에게 플레이어 공격 함수 호출
                    BattleManager.Instance.ExecutePlayerAttack(_playerStat, slime);
                }
                break;
            }
        }
    }

    // 아이템 줍기 함수
    private void TryPickupItem()
    {
        // 내 주변 반지름(_pickupRange) 이내의 모든 콜라이더 영역 검사 저장
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _pickupRange);

        foreach (Collider hitCollider in hitColliders) // 하나씩 꺼내서 확인
        {
            // 만약 오브젝트의 태그가 "Item" 이라면
            if (hitCollider.CompareTag("Item") == true)
            {
                // 필드 아이템 컴포넌트에서 데이터 가져와 저장
                FieldItem fieldItem = hitCollider.GetComponent<FieldItem>();

                // 애니메이션 컨트롤러 있으면
                if (fieldItem != null)
                {
                    if (_animatorController != null)
                    {
                        // 줍기 애니메이션 실행
                        _animatorController.SetState(AllState.Drop);
                    }
                }

                // 인벤토리 매니저가 있으면
                if (_inventoryManager != null)
                {
                    // 인벤토리 함수 호출
                    _inventoryManager.AddPotion();
                }

                // 오브젝트 비활성화 및 파괴
                hitCollider.gameObject.SetActive(false);
                Destroy(hitCollider.gameObject);

                break;
            }
        }
    }

    // 피격시 경직 함수
    public void TriggerHitReaction()
    {
        _stunTimer = _hitStunDuration; // 경직 타이머 가동
        _moveDirection = Vector3.zero; // 이동 정지

        if (_animatorController != null) // 애니메이션 컨트롤러 있으면
        {
            _animatorController.SetState(AllState.Hit); // 피격 애니메이션 재생
        }
    }

    // 범위 기즈모로 그리기 함수
    private void OnDrawGizmosSelected()
    {
        // 1. 줍기 범위 (노란색 선 원형)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _pickupRange);

        // 2. 공격 범위 (빨간색 선 원형)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

