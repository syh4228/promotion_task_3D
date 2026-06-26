using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot; 
    [SerializeField] private Button Button_UseSelectItem;
    [SerializeField] private Button Button_CloseSelf;
    // [SerializeField] private UIButton Button_CloseSelfAllArea;

    private Dictionary<long, InventorySlotUI> _itemSlotList = new Dictionary<long, InventorySlotUI>();

    private long _currentSelectedItemUniqueId; 

    private void OnEnable()
    {
        if (Button_UseSelectItem != null)
        {
            Button_UseSelectItem.onClick.RemoveAllListeners();
            Button_UseSelectItem.onClick.AddListener(OnClick_UseSelectItem);
        }
        else
        {
            Debug.LogError("InventoryUI: Button_UseSelectItem이 연결되지 않았습니다! 인스펙터를 확인하세요.");
        }
        // Button_CloseSelfAllArea.BindOnClickButtonEvent(OnClick_ClosePopup);

        if (Button_CloseSelf != null)
        {
            Button_CloseSelf.onClick.RemoveAllListeners();
            Button_CloseSelf.onClick.AddListener(OnClick_ClosePopup);
        }

        SetInventoryItemSlotOnEnable();
        ActiveUseSelectItemButton(false);

        GameManager.Inst.Inventory.OnItemAdded += CreateSlot;
        GameManager.Inst.Inventory.OnItemUpdated += RefreshSlot;
    }

    private void OnDisable()
    {
        GameManager.Inst.Inventory.OnItemAdded -= CreateSlot;
        GameManager.Inst.Inventory.OnItemUpdated -= RefreshSlot;
    }

    private void SetInventoryItemSlotOnEnable()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (_itemSlotList.Count > 0)
        {
            foreach (var slot in _itemSlotList)
            {
                DestroyImmediate(slot.Value.gameObject);
            }
            _itemSlotList.Clear();
        }

        var itemList = InventoryManager.Instance.GetPlayerItemList();

        Debug.Log($"인벤토리 아이템 개수 : {itemList.Count}");

        foreach (var item in itemList)
        {
            Debug.Log($"아이템 발견 : {item.ItemDataId}");
        }

        if (itemList == null || itemList.Count == 0)
        {
            return;
        }

        foreach (var itemModel in itemList)
        {
            CreateSlot(itemModel.ItemUniqueId, itemModel.ItemDataId, itemModel.ItemStackCount);
        }
    }

    public void OnClick_ClosePopup()
    {
        this.gameObject.SetActive(false);
    }


    public void OnClick_UseSelectItem()
    {
        RequestSelectedUseItem();
    }

    private void RequestSelectedUseItem()
    {
        long idToUse = _currentSelectedItemUniqueId;

        bool isItemRemoved = InventoryManager.Instance.RequestUseItem(idToUse);

        if (isItemRemoved == true)
        {
            _currentSelectedItemUniqueId = 0;
            ActiveUseSelectItemButton(false);
        }
    }

    private void ActiveUseSelectItemButton(bool isActive)
    {
        if (Button_UseSelectItem != null)
        {
            Button_UseSelectItem.gameObject.SetActive(isActive);
        }
    }

    private void RemoveItemSlot(long removedItemUniqueId)
    {
        if (_itemSlotList.ContainsKey(removedItemUniqueId) == false)
        {
            Debug.LogError("이상합니다! 제거가 된 아이템을 슬롯을 찾을수가 없네요!");
            return;
        }

        var slotComponent = _itemSlotList[removedItemUniqueId];
        _itemSlotList.Remove(removedItemUniqueId);
        Destroy(slotComponent.gameObject);
    }


    private void CreateSlot(long itemUniqueId, string itemDataId, int itemStackCount)
    {
        Debug.Log($"슬롯 생성을 시도합니다: {itemDataId}");

        var gObj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<InventorySlotUI>();
        if (slotComponent == null) return;


        slotComponent.InitSlot(itemUniqueId, itemDataId, itemStackCount);
        slotComponent.gameObject.name = $"ItemSlot : {slotComponent.SlotItemUniqueId}";

        _itemSlotList.Add(slotComponent.SlotItemUniqueId, slotComponent);
        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);
    }


    private void OnChildSlotSelected(long selectedItemUniqueId)
    {
        foreach (var slotKv in _itemSlotList)
        {
            var slot = slotKv.Value;
            bool isSlotSelected = (selectedItemUniqueId == slot.SlotItemUniqueId);
            slot.ChangeSelectedState(isSlotSelected);

            if (isSlotSelected == true)
            {
                _currentSelectedItemUniqueId = slot.SlotItemUniqueId;
                ActiveUseSelectItemButton(slot.IsUsableItem);
            }
        }
        Debug.LogWarning($"자식 슬롯 {selectedItemUniqueId} 선택됨!");
    }

    private void RefreshSlot(long uniqueId)
    {
        if (!_itemSlotList.ContainsKey(uniqueId)) return;

        var item = InventoryManager.Instance.GetPlayerItemList().Find(i => i.ItemUniqueId == uniqueId);

        if (item == null)
        {
            RemoveItemSlot(uniqueId);
        }
        else
        {
            // 있으면 수량만 갱신
            _itemSlotList[uniqueId].InitSlot(item.ItemUniqueId, item.ItemDataId, item.ItemStackCount);
        }
    }
}
