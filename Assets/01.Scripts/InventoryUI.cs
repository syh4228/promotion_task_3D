using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private UIButton Button_UseSelectItem;
    [SerializeField] private UIButton Button_CloseSelf;
    [SerializeField] private UIButton Button_CloseSelfAllArea;

    private Dictionary<long, InventorySlotUI> _itemSlotList = new Dictionary<long, InventorySlotUI>();

    private long _currentSelectedItemUniqueId; 

    private void OnEnable()
    {
        Button_UseSelectItem.BindOnClickButtonEvent(OnClick_UseSelectItem, true);
        Button_CloseSelf.BindOnClickButtonEvent(OnClick_ClosePopup);
        Button_CloseSelfAllArea.BindOnClickButtonEvent(OnClick_ClosePopup);
        
        SetInventoryItemSlotOnEnable();
        ActiveUseSelectItemButton(false);
    }

    private void OnDisable()
    {
        Button_UseSelectItem.UnBindAllOnClickButtonEvent();
    }

    private void SetInventoryItemSlotOnEnable()
    {
        if (_itemSlotList.Count > 0)
        {
            foreach (var slot in _itemSlotList)
            {
                DestroyImmediate(slot.Value.gameObject);
            }
            _itemSlotList.Clear();
        }

        var itemList = InventoryManager.Instance.GetPlayerItemList();

        if (itemList == null || itemList.Count == 0)
        {
            Debug.LogWarning("보유한 아이템이 없습니다!");
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
        bool isItemRemoved = InventoryManager.Instance.RequestUseItem(_currentSelectedItemUniqueId);

        if (isItemRemoved == true)
        {
            RemoveItemSlot(_currentSelectedItemUniqueId);
            _currentSelectedItemUniqueId = 0;
            ActiveUseSelectItemButton(false);
        }
    }

    private void ActiveUseSelectItemButton(bool isActive)
    {
        Button_UseSelectItem.gameObject.SetActive(isActive);
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
}
