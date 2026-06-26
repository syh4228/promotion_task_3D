using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private Button Button_Slot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Selected;

    private event Action<long> OnSelectEvent;

    public long SlotItemUniqueId { get; private set; }
    public bool IsUsableItem { get; private set; }

    private void OnEnable()
    {
        Image_Selected.gameObject.SetActive(false);

        Button_Slot.onClick.RemoveAllListeners();
        Button_Slot.onClick.AddListener(OnClick_SelectItem);
    }

    public void SetIcon(string itemDataId, int itemCount)
    {
        var itemData = GameManager.Inst.Data.GetItemData(itemDataId);

        if (itemData == null)
        {
            Debug.LogWarning($"Item 데이터를 불러올 수 없습니다! 경로:{itemDataId}");
            return;
        }

        string iconPath = itemData.IconPath;

        if (string.IsNullOrEmpty(iconPath) == true)
        {
            Debug.LogWarning($"Item 데이터에 아이콘 경로가 존재하지 않습니다.");
            return;
        }

        IsUsableItem = (itemData.ItemType == "Potion" || itemData.ItemType == "Buff");


        GameUtil.LoadAndSetSpriteImage(Image_Icon, iconPath).Forget();

        Text_StackCount.text = $"{itemCount}";
    }

    private void OnDisable()
    {
        OnSelectEvent = null;
    }

    public void InitSlot(long slotUniqueId, string itemDataId, int itemStackCount)
    {
        SlotItemUniqueId = slotUniqueId;
        SetIcon(itemDataId, itemStackCount);
    }

    public void OnClick_SelectItem()
    {
        OnSelectEvent?.Invoke(SlotItemUniqueId);

        Debug.Log($"{SlotItemUniqueId}눌러졌다");
    }

    public void BindSlotSelectEvent(Action<long> onSelectEvent)
    {
        OnSelectEvent = onSelectEvent;
    }

    public void ChangeSelectedState(bool isSelected)
    {
        if (Image_Selected != null)
        {
            Image_Selected.gameObject.SetActive(isSelected);
        }
    }
}
