using UnityEngine;
using System;
using System.Collections.Generic;

public enum AllState
{
    Idle,
    Walk,
    Run,
    Attack,
    Dead
}
public class Animatorcontroller : MonoBehaviour
{
    [SerializeField] private Animator Animator_Control; // 애메이터 연결

    private AllState _currentState; // 현재 상태

    // 딕셔너리로 모든 애니메이션 행동 저장
    private Dictionary<AllState, Action> _animationActions;

    // 애니메이션 상태를 해시 숫자로 저장
    private static readonly int AnimationIsRun = Animator.StringToHash("IsRun");
    private static readonly int AnimationIsDead = Animator.StringToHash("IsDead");
    private static readonly int AnimationIsAttack = Animator.StringToHash("IsAttack");
    private static readonly int AnimationIsWalk = Animator.StringToHash("ISWalk");

    private void Awake()
    {
        // 각 상태에 맞는 행동을 딕셔너리에 저장
        _animationActions = new Dictionary<AllState, Action>();

        // 각 상태에 따른 애니메이션 실행 함수 연결
        _animationActions.Add(AllState.Idle, AllIdleAnimation);
        _animationActions.Add(AllState.Walk, AllWalkAnimation);
        _animationActions.Add(AllState.Run, AllRunAnimation);
        _animationActions.Add(AllState.Dead, AllDeadAnimation);
        _animationActions.Add(AllState.Attack, AllAttackAnimation);
    }

    private void Start()
    {
        if (Animator_Control != null) // 애니메이션 컨트롤러가 없으면
        {
            // 로그로 알림
            Debug.LogError("애니메이터가 연결되지 않았습니다! 확인해주세요.");
        }
    }

    // 행동변화 함수
    public void SetState(AllState newState)
    {
        // 새 행동이 현재행동 과 같으면
        if (newState == _currentState)
        {
            // 새 행동이 공격이 아니면
            if ( newState != AllState.Attack)
            {
                return; 
            }
        }

        // 딕셔너리에서 새 행동이 있는지 찾고, 액션에 담기
        if (_animationActions.TryGetValue(newState, out Action action))
        {
            _animationActions[newState].Invoke(); // 새 행동 실행
            _currentState = newState; // 현재 행동으로 저장
        }
        else // 못 찾았으면
        {
            Debug.LogWarning($"{newState} 연결된 애니메이션이 없습니다.");
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
        Animator_Control.SetBool(AnimationIsWalk, true);
    }

    private void AllRunAnimation() // 달리기 애니메이션 함수
    {
        ResetAllBoolParameters();
        // 달리기 애니메이션 실행
        Animator_Control.SetBool(AnimationIsRun, true);
    }

    private void AllDeadAnimation() // 죽음 애니메이션 함수
    {
        ResetAllBoolParameters();
        // 죽음 애니메이션 실행
        Animator_Control.SetBool(AnimationIsDead, true);
    }

    private void AllAttackAnimation() // 공격 애니메이션 함수
    {
        ResetAllBoolParameters();
        // 공격 애니메이션 실행
        Animator_Control.SetBool(AnimationIsAttack, true);
    }

    // 상태 초기화 함수
    private void ResetAllBoolParameters()
    {
        // 걷기 애니메이션 끄기
        Animator_Control.SetBool(AnimationIsWalk, false);
        // 달리기 애니메이션 끄기
        Animator_Control.SetBool(AnimationIsRun, false);
        // 죽음 애니메이션 끄기
        Animator_Control.SetBool(AnimationIsDead, false);
        // 공격 애니메이션 끄기
        Animator_Control.SetBool(AnimationIsAttack, false);
    }
}
