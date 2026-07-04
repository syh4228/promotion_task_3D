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

    public event System.Action<long, string, int> OnItemAdded;
    public event System.Action<long> OnItemUpdated;
    // 아이템 사용됬는지 확인 이벤트
    public event System.Action<ItemData> OnItemUsed;

    // 인벤토리 UI에 리스트 넘겨주는 함수
    public List<ItemModel> GetPlayerItemList()
    {
        return _playerInventory;
    }

    // 플레이어가 아이템을 주웠을떄 호출하는 함수
    public void AddItem(string itemDataId, int count)
    {
        // 같은 itemDataId 확인한 후 itemDataId  저장
        ItemModel existingItem = FindItemByDataId(itemDataId);

        // 최대 보유 개수 저장
        int maxStackCount = int.MaxValue;
        // 게임매니저가 있으면 게임매니저에서 아이템 데이터가져와 저장
        ItemData itemData = GameDataManager.Instance != null ? GameDataManager.Instance.GetItemData(itemDataId) : null;

        // 아이템 데이터가 있고, 아이템 최대 보유갯수가 0 보다 크면
        if (itemData != null && itemData.MaxStackCount > 0)
        {
            // 아이템 최대 보유 갯수 저장
            maxStackCount = itemData.MaxStackCount;
        }

        // 같은 itemDataId면
        if (existingItem != null)
        {
            // 아이템 보유 갯수 추가 후 저장
            existingItem.ItemStackCount = existingItem.ItemStackCount + count;

            // 만약 아이템 보유 갯수가 최대 보유 갯수보다 많으면
            if (existingItem.ItemStackCount > maxStackCount)
            {
                // 최대 보유 갯수로 저장
                existingItem.ItemStackCount = maxStackCount;
            }

            Debug.Log($"[InventoryManager] 기존 아이템에 겹침: {itemDataId} (현재 {existingItem.ItemStackCount}개)");

            // 새로만들지 않고 갱신
            OnItemUpdated?.Invoke(existingItem.ItemUniqueId);
            return;
        }

        // 처음 획득하는 아이템일 때만 새 슬롯 생성
        ItemModel newItem = new ItemModel
        {
            ItemUniqueId = DateTime.UtcNow.Ticks, // 고유번호 발급
            ItemDataId = itemDataId,
            ItemStackCount = Mathf.Min(count, maxStackCount)
        };

        _playerInventory.Add(newItem);
        Debug.Log($"[InventoryManager] 가방에 아이템 추가됨: {itemDataId}");

        OnItemAdded?.Invoke(newItem.ItemUniqueId, newItem.ItemDataId, newItem.ItemStackCount);
    }

    // 같은 도감 번호를 가진 아이템이 있는지 찾는 함수
    private ItemModel FindItemByDataId(string itemDataId)
    {
        // 플레이어 인벤토이에 아이템 하나씩 검사
        foreach (ItemModel item in _playerInventory)
        {
            // 만약 아이템데이터 id가 같으면
            if (item.ItemDataId == itemDataId)
            {
                return item; // 아이템 반환
            }
        }

        return null;
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

        // 피드백 반영 이 아이템이 사용됬다고만 알림
        OnItemUsed?.Invoke(itemData);

        // 사용한 아이템 처리
        targetItem.ItemStackCount--;

        if (targetItem.ItemStackCount <= 0)
        {
            _playerInventory.Remove(targetItem);
        }

        OnItemUpdated?.Invoke(uniqueId);

        return true; // 성공적으로 사용하고 지웠음을 UI에게 알림
    }
}
