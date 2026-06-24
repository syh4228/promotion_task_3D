using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable] // 인벤토리 실제 저장 데이터
public class ItemModel
{
    public long ItemUniqueId;   // 고유 발급 번호 
    public string ItemDataId;   // 도감 번호 ("Item_RedVial")
    public int ItemStackCount;  // 겹쳐진 갯수
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<ItemModel> _playerInventory = new List<ItemModel>();

    private void Awake()
    {
        Instance = this;
    }

    // 인벤토리 UI에 리스트 넘겨주는 함수
    public List<ItemModel> GetPlayerItemList()
    {
        return _playerInventory;
    }

    // 플레이어가 아이템을 주웠을떄 호출하는 함수
    public void AddItem(string itemDataId, int count)
    {
        ItemModel newItem = new ItemModel
        {
            ItemUniqueId = DateTime.UtcNow.Ticks, // 고유번호 발급
            ItemDataId = itemDataId,
            ItemStackCount = count
        };

        _playerInventory.Add(newItem);
        Debug.Log($"[InventoryManager] 가방에 아이템 추가됨: {itemDataId}");

        UpdateUI(); // UI 갱신
    }

    // 인벤토리 UI에서 아이템 사용하면 호출하는 함수
    public bool RequestUseItem(long uniqueId)
    {
        // 가방에서 그 고유번호를 가진 아이템을 찾기
        ItemModel targetItem = _playerInventory.Find(item => item.ItemUniqueId == uniqueId);

        if (targetItem == null) return false;

        // 게임데이터매니저에서 아이템 상세 정보 가져오기
        ItemData itemData = GameDataManager.Instance.GetItemData(targetItem.ItemDataId);

        if (itemData == null) return false;

        // ItemType에 따라 효과 적용
        if (itemData.ItemType == "Potion")
        {
            // 배틀 매니저를 통해 회복 실행
            if (itemData.Heal > 0 && BattleManager.Instance != null)
            {
                BattleManager.Instance.RequestPlayerHeal(itemData.Heal);
                Debug.Log($"[InventoryManager] {itemData.Name} 사용! 체력 {itemData.Heal} 회복!");
            }
        }
        else if (itemData.ItemType == "Buff")
        {
            // 플레이어 이동속도 버프 함수 호출
            PlayerController player = FindFirstObjectByType<PlayerController>();

            if (player != null && itemData.Speed > 0)
            {
                player.ApplySpeedBuff(itemData.Speed, 5.0f);
                Debug.Log($"[InventoryManager] {itemData.Name} 사용! 5초간 스피드 {itemData.Speed} 증가!");
            }
        }

        // 사용한 아이템 처리
        targetItem.ItemStackCount--;

        if (targetItem.ItemStackCount <= 0)
        {
            _playerInventory.Remove(targetItem);
        }

        UpdateUI(); // 아이템을 썼으니 UI 갱신
        return true; // 성공적으로 사용하고 지웠음을 UI에게 알림
    }

    // UI 업데이트
    private void UpdateUI()
    {
        
    }
}
