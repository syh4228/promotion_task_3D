using UnityEngine;
using UnityEngine.AI;

public class SlimeAi : MonoBehaviour
{
    [Header("몬스터 설정")]
    [SerializeField] private float _moveSpeed = 3.5f; //이동 속도
    [SerializeField] private float _detectionRange = 10.0f; // 플레이어 추적 범위 (초록색 원)
    [SerializeField] private float _attackRange = 2.0f;     // 공격 범위 (빨간색 원)
    [SerializeField] private float _attackCooldown = 1.5f;  // 공격 쿨타임

    [Header("컴포넌트 연결")]
    [SerializeField] private Animatorcontroller _animatorController; // 애니메이션 컨트롤러 연결

    private Transform _player; // 플레이어 위치 저장
    private NavMeshAgent _agent; // 네비게이션 정보 저장
    private float _cooldownTimer = 0f; // 쿨타임 타이머

    private void Start()
    {
        // 플레이어 태그를 가진 오브젝트 정보 가져와 저장
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null) // 플레이어 오브젝트 있으면
        {
            _player = playerObj.transform; // 위치 저장
        }

        // 네비게이션 정보 가져와 저장
        _agent = GetComponent<NavMeshAgent>();

        if (_agent != null) // 정보 있으면
        {
            _agent.speed = _moveSpeed; // 이동 속도 저장
        }

        if (_animatorController == null) // 애니메이션 컨트롤러 없으면
        {
            // 애니메이션 컨트롤러 가져와 저장
            _animatorController = GetComponent<Animatorcontroller>();
        }
    }

    private void Update()
    {
        // 플레이어가 없고, 네이게이션 정보가 없으면 반환
        if (_player == null || _agent == null) return;

        if (_cooldownTimer > 0f) // 쿨타임이 0보다 크면
        {
            // 쿨타임 감소
            _cooldownTimer = _cooldownTimer - Time.deltaTime;
        }

        // 플레이어 위치 백터3로 받아 저장
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _attackRange) // 플레이어 위치가 공격범위 안이면
        {
            _agent.isStopped = true; // 추적 정지
            _agent.velocity = Vector3.zero;  // 이동 0으로 고정

            RotateTowardsPlayer(); // 플레이어 바라보게 하는 함수 호출

            if (_cooldownTimer <= 0f) // 쿨타임이 0보다 적으면
            {
                ExecuteAttack(); // 공격 함수 호출
            }
        }
        else if (distanceToPlayer <= _detectionRange) // 플레이어가 추적 범위 안이면
        {
            _agent.isStopped = false; // 추적 시작
            _agent.SetDestination(_player.position); // 플레이어 위치까지 이동

            if (_animatorController != null) // 만약 애니메이션 컨트롤러 있으면
            {
                _animatorController.SetState(AllState.Run); // 달리기 애니메이션 재생
            }
        }
        else // 아니면
        {
            _agent.isStopped = true; // 추적 정지
            _agent.velocity = Vector3.zero; // 이동 0 고정

            if (_animatorController != null) // 애니메이션 컨트롤러 있으면
            {
                _animatorController.SetState(AllState.Idle); // 대기 애니메이션 재생
            }
        }
    }

    // 플레이어 바라보게 하는 함수
    private void RotateTowardsPlayer()
    {
        // 플레이어가 바라보는 방향을 저장
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0; // Y축 위치 고정

        if (direction.magnitude > 0.1f) // 저장된 위치의 백터 길이가 0.1f 보다 적으면
        {
            // 회전 각도 계산
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
        }
    }

    // 공격 함수
    private void ExecuteAttack()
    {
        if (_animatorController != null) // 애니메이션 컨트롤러 있으면
        {
            _animatorController.SetState(AllState.Attack);  // 애니메이션 공격 재생
        }
        
        // 쿨타임 시간 초기화
        _cooldownTimer = _attackCooldown; 
    }

    // 범위 기즈모로 그리는 함수
    private void OnDrawGizmosSelected()
    {
        // 추적 범위는 초록색 선 원형으로 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // 공격 범위는 빨간색 선 원형으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
