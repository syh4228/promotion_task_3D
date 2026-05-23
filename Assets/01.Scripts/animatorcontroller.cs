using UnityEngine;
using System;
using System.Collections.Generic;

public enum AllState
{
    Idle,
    Walk,
    Run,
    Attack,
    Jump
}

public class Animatorcontroller : MonoBehaviour
{
    [SerializeField] private Animator Animator_Control;

    private AllState _currentState;

    private Dictionary<AllState, Action> _animationActions;

    private static readonly int AnimationIsWalk = Animator.StringToHash("IsWalk");
    private static readonly int AnimationIsRun = Animator.StringToHash("IsRun");
    private static readonly int AnimationIsAttack = Animator.StringToHash("IsAttack");
    private static readonly int AnimationIsJump = Animator.StringToHash("IsJump");

    private void Awake()
    {
        _animationActions = new Dictionary<AllState, Action>();

        _animationActions.Add(AllState.Idle, AllIdleAnimation);
        _animationActions.Add(AllState.Walk, AllWalkAnimation);
        _animationActions.Add(AllState.Run, AllRunAnimation);
        _animationActions.Add(AllState.Attack, AllAttackAnimation);
    }

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

        ResetAllBoolParameters();
    }

    public void SetState(AllState newState)
    {
        // 새 행동이 현재행동 과 같으면
        if (newState == _currentState)
        {
            // 새 행동이 공격이 아니면
            if (newState != AllState.Attack)
            {
                return;
            }
        }

        // 딕셔너리에서 새 행동이 있는지 찾고, 액션에 담기
        if (_animationActions.TryGetValue(newState, out Action action))
        {
            action.Invoke(); // 새 행동 실행
            _currentState = newState; // 현재 행동으로 저장
        }
        else
        {
            Debug.LogWarning($"{newState} 연결된 애니메이션이 없습니다.");
        }
    }

    public void SetJump(bool isJump) // 점프 애니메이션 함수
    {
        if (Animator_Control != null)
        {
            SafeSetBool(AnimationIsJump, isJump);
        }
    }

    private void AllIdleAnimation() // 대기애니메이션 함수
    {
        // 상태 초기화 함수 호출
        ResetAllBoolParameters();
    }

    private void AllWalkAnimation() // 걷는 애니매이션 함수
    {
        ResetAllBoolParameters();
        // 걷기 애니메이션 실행
        SafeSetBool(AnimationIsWalk, true);
    }

    private void AllRunAnimation() // 달리기 애니메이션 함수
    {
        ResetAllBoolParameters();
        // 달리기 애니메이션 실행
        SafeSetBool(AnimationIsRun, true);
    }

    private void AllAttackAnimation() // 공격 애니메이션 함수
    {

        ResetAllBoolParameters();
        // 공격 애니메이션 실행
        SafeSetBool(AnimationIsAttack, true);
    }

    // 상태 초기화 함수
    private void ResetAllBoolParameters()
    {

        // 걷기 애니메이션 끄기
        SafeSetBool(AnimationIsWalk, false);
        // 달리기 애니메이션 끄기
        SafeSetBool(AnimationIsRun, false);
        // 공격 애니메이션 끄기
        SafeSetBool(AnimationIsAttack, false);
        SafeSetBool(AnimationIsJump, true);
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
}
