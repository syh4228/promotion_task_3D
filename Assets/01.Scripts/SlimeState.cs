using UnityEngine;

public class SlimeState : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int _maxHp = 50; // 슬라임이니까 체력을 조금 낮게
    [SerializeField] private int _currentHp;

    [Header("공격력 설정")]
    [SerializeField] private int _atk = 5; // 슬라임 공격력

    [Header("컴포넌트 연결")]
    [SerializeField] private Animatorcontroller _animatorController;

    // 사망 알리기
    public bool IsDead
    {
        get { return _currentHp <= 0; }
    }

    // 공격력 알리기
    public int AttackPower
    {
        get { return _atk; }
    }

    private void Start()
    {
        // 최대체력으로 설정
        _currentHp = _maxHp;
    }

    // 대미지 받아오는 함
    public void ApplyDamage(int finalDamage)
    {
        if (IsDead == true) return; // 사망했으면 반환

        _currentHp = _currentHp - finalDamage; // 현재체력에 대미지 감소
        Debug.Log($"슬라임이 데미지 {finalDamage} 받음. 현재 체력: {_currentHp}");

        if (_currentHp <= 0) // 체력이 0이하면
        {
            _currentHp = 0; // 체력 0으로 수정
            Die(); // 사망함수 호출
        }
        else // 사망이 아니면
        {
            // 슬라임 AI 컴포넌트 가져와 저장
            SlimeAi myAi = GetComponent<SlimeAi>();

            if (myAi != null) // 슬라임 AI가 있다면
            {
                // 피격시 경직 함수 호출
                myAi.TriggerHitReaction();
            }
        }
    }

    // 사망함수
    private void Die()
    {
        Debug.Log("슬라임 처치");

        // 애니메이션 컨트롤러 있으면
        if (_animatorController != null)
        {
            // 사망 애니메이션 재생
            _animatorController.SetState(AllState.Dead);
        }

        // 콜라이더 컴포넌트 가져와 저장
        Collider collider = GetComponent<Collider>();

        if (collider != null) // 콜라이더 있으면 
        {
            collider.enabled = false; // 비활성화
        }

        Destroy(gameObject, 2f); // 2초뒤 오브젝트 파괴
    }
}
