using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int _maxHp = 100; // 최대 체력
    [SerializeField] private int _currentHp; // 현재 체력

    [Header("공격력 설정")]
    [SerializeField] private int _atk = 10; // 공격력

    [Header("컴포넌트 연결")]
    [SerializeField] private Animatorcontroller _animatorController; // 애니메이션 컨트롤러 연결

    public bool IsDead // 외부에 죽었을시 알리기
    {
        get
        {
            // 체력 0이하면 반환
            return _currentHp <= 0;
        }
    }

    public int AttackPower // 외부에 공격력 알리기
    {
        get
        {
            return _atk;
        }
    }

    private void Start()
    {
        // 시작시 체력 최대체력으로 
        _currentHp = _maxHp;
    }

    // 대미지 반영 함수
    public void ApplyDamage(int finalDamage)
    {
        if (IsDead == true) return; // 죽었으면 반환

        // 현재체력에 배틀매니저에서 보내준 대미지를 깍은체력을 현제 체력으로
        _currentHp = _currentHp - finalDamage;
        Debug.Log($"데미지 {finalDamage} 받음. 현재 체력: {_currentHp}");

        if (_currentHp <= 0 ) // 현재체력이 0이하면
        {
            _currentHp = 0; // 현제체력 0으로 수정
            Die(); // 죽음 함수 호출
        }
    }

    // 회복 반영 함수
    public void ApplyHeal(int finalHeal)
    {
        if (IsDead == true) return; // 죽었으면 반환

        // 배틀매니저에서 보내준 회복량 현제체력에 더하기
        _currentHp = _currentHp + finalHeal;

        if (_currentHp > _maxHp) // 만약 최대체력보다 현재체력이 많으면
        {
            _currentHp = _maxHp; // 최대체력으로 수정
        }

        Debug.Log($"체력 {finalHeal} 회복. 현재 체력: {_currentHp}");
    }

    // 사망 처리 함수
    private void Die()
    {
        Debug.Log("플레이어 사망!");

        if (_animatorController != null) // 애니메이션 컨트롤러 있으면
        {
            _animatorController.SetState(AllState.Dead); // 사망 애니메이션 재생
        }
        else
        {
            Debug.LogWarning("애니메이터 컨트롤러가 연결되지 않았습니다");
        }
    }
}
