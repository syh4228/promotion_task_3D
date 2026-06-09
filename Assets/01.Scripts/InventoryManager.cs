using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("아이템 수량 관리")]
    [SerializeField] private int _potionCount = 0; // 먹은 포션 개수 확인

    // 포션을 주웠을때 불리는 함수
    public void AddPotion()
    {
        _potionCount = _potionCount + 1; // 갯수 카운트 + 1
        Debug.Log($"[Inventory] 포션 획득! 현재 소지 개수: {_potionCount}");

        // 배틀매니저가 있으면
        if (BattleManager.Instance != null)
        {
            // 배틀매니저에 체력회복 50 알리기
            BattleManager.Instance.RequestPlayerHeal(50);
        }
        else
        {
            Debug.LogWarning("씬에 BattleManager를 찾을 수 없어 회복 처리를 생략합니다.");
        }
    }
}
