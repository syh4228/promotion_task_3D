using UnityEngine;


public class Animatorcontroller : MonoBehaviour
{
    [SerializeField] private Animator Animator_Control;

    private AllState _currentState;

    // 붙 타임 해시 파라미터
    private static readonly int AnimationIsRun = Animator.StringToHash("IsRun");
    private static readonly int AnimationIsDead = Animator.StringToHash("IsDead");
    private static readonly int AnimationIsJump = Animator.StringToHash("IsJump");

    // 트리거 타입 해시 파라미터
    private static readonly int AnimationIsRoll = Animator.StringToHash("IsRoll");
    private static readonly int AnimationIsAttack = Animator.StringToHash("IsAttack");
    private static readonly int AnimationTriggerHit = Animator.StringToHash("IsHit");
    private static readonly int AnimationTriggerTalk = Animator.StringToHash("IsTalk");
    private static readonly int AnimationTriggerDrop = Animator.StringToHash("IsDrop");



    private void Start()
    {
        // 시작할때 만약 연결된 애니메이션이 없다면
        if (Animator_Control == null)
        {
            // 애니메이션 컴포넌트에서 애니메이션을 가져오고
            Animator_Control = GetComponent<Animator>();

            // 만약 애니메이션을 못찾으면
            if (Animator_Control == null)
            {
                // 디버그 로그 띄우기
                Debug.LogError("애니메이터가 연결되지 않았습니다! 확인해주세요.");
            }
        }
    }

    public void SetState(AllState newState)
    {
        // 새 행동이 현재행동 과 같으면
        if (newState == _currentState)
        {
            // 새 행동이 공격이나, 피격이 아니면
            if (newState != AllState.Attack && newState != AllState.Hit)
            {
                return;
            }
        }

        // 기존 상태 초기화 함수 호출
        ResetAllBoolParameters();

        switch (newState)
        {
            case AllState.Idle: // 대기는 기본 상태
                break;
            case AllState.Run:
                SafeSetBool(AnimationIsRun, true);
                break;
            case AllState.Dead:
                SafeSetBool(AnimationIsDead, true);
                break;
            case AllState.Attack:
                SafeSetTrigger(AnimationIsAttack);
                break;
            case AllState.Hit:
                SafeSetTrigger(AnimationTriggerHit);
                break;
            case AllState.Roll:
                SafeSetTrigger(AnimationIsRoll);
                break;
            case AllState.Talk:
                SafeSetTrigger(AnimationTriggerTalk);
                break;
            case AllState.Drop:
                SafeSetTrigger(AnimationTriggerDrop);
                break;
            default:
                break;
        }
    }

    private void ResetAllBoolParameters()
    {
        // 달리기 애니메이션 끄기
        SafeSetBool(AnimationIsRun, false);
    }

    public void SetJump(bool isJump)
    {
        if (Animator_Control != null)
        {
            // isGrounded를 스스로 끄거나 킴
            SafeSetBool(AnimationIsJump, isJump);
        }
    }

    // 애니메이션 파라미터 조정 함수
    private void SafeSetBool(int parameterHash, bool value)
    {
        if (Animator_Control == null) // 애니메이션 컨트롤러가 없으면
        {
            return; // 반환
        }

        // 애니메이터가 가진 파라미터 목록을 하니씩 검사
        foreach (AnimatorControllerParameter param in Animator_Control.parameters)
        {
            // 해시 번호 바꾸려는 번호와 맞는지 체크
            if (param.nameHash == parameterHash)
            {
                // 스위치가 실제로 있을때만 값 변경
                Animator_Control.SetBool(parameterHash, value);
                return; // 반환
            }
        }
    }

    // 애니메이션 트리거 조작 함수
    private void SafeSetTrigger(int parameterHash)
    {
        if (Animator_Control == null) return;

        foreach (AnimatorControllerParameter param in Animator_Control.parameters)
        {
            if (param.nameHash == parameterHash)
            {
                Animator_Control.SetTrigger(parameterHash);
                return;
            }
        }
    }
}
