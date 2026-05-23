using UnityEngine;
using System;

public class Groundcheck : MonoBehaviour
{
    // 지면 트리거 이벤트의 상태가 변할때 마다 현재 상태를 알려줌
    public event Action<bool> GroundTriggeredEvent;

    // 다른 콜라이어가 내 트리거 영역 안에 있으면 작동
    private void OnTriggerStay(Collider other)
    {
        GroundTriggeredEvent.Invoke(true); // 트리거 true
    }

    // 다른 콜라이더가 내 트리거 영역에서 나가면 작동
    private void OnTriggerExit(Collider other)
    {
        GroundTriggeredEvent.Invoke(false); // 트리거 false
    }
}
