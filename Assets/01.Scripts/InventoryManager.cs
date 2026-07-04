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
        Debug.Log("AddItem 호출됨");

        ItemModel newItem = new ItemModel
        {
            ItemUniqueId = DateTime.UtcNow.Ticks, // 고유번호 발급
            ItemDataId = itemDataId,
            ItemStackCount = count
        };

        _playerInventory.Add(newItem);
        Debug.Log($"[InventoryManager] 가방에 아이템 추가됨: {itemDataId}");

        Debug.Log("이벤트 발생");
        OnItemAdded?.Invoke(newItem.ItemUniqueId, newItem.ItemDataId, newItem.ItemStackCount);
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
