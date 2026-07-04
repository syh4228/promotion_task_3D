using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _priceText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _selectButton; 
    [SerializeField] private Image _selectedImage;

    // 슬롯에 연결된 뷰모델 저장
    private ShopItemViewModel _boundItem;
    // 선택된 슬롯 알려주는 이벤트
    private Action<ShopItemViewModel> _onSelectEvent;

    private void OnEnable()
    {
        if (_selectedImage != null) 
        {
            // 이전 선택 상태가 남아있지 않도록 비활성화
            _selectedImage.gameObject.SetActive(false);
        }

        if (_selectButton != null)
        {
            // 기존에 등록된 이벤트가 있다면 제거
            _selectButton.onClick.RemoveAllListeners();
            // 클릭 이벤트 등록
            _selectButton.onClick.AddListener(OnClickSelect);
        }
    }

    private void OnDisable()
    {
        // 부모 View를 참조하는 이벤트 제거
        _onSelectEvent = null;
    }

    // 실제 UI(Text, Image)에 표시
    public void Bind(ShopItemViewModel itemViewModel, Action<ShopItemViewModel> onSelectEvent)
    {
        // 현재 슬롯이 표현할 데이터 저장
        _boundItem = itemViewModel;
        // 선택 이벤트 저장
        _onSelectEvent = onSelectEvent;

        if (_nameText != null)
        {
            // 아이템 이름 저장
            _nameText.text = itemViewModel.ItemName;
        }

        if (_priceText != null)
        {
            // 아이템 금액 저장
            _priceText.text = itemViewModel.Price + " G";
        }

        // 아이콘 이미지가 있고, 아이콘 경로가 있을경우
        if (_iconImage != null && string.IsNullOrEmpty(itemViewModel.IconPath) == false)
        {
            // 아이콘 이미지 로드
            GameUtil.LoadAndSetSpriteImage(_iconImage, itemViewModel.IconPath).Forget();
        }
    }

    // 아이템 ID 반환 함수
    public string GetItemId()
    {
        if (_boundItem == null)
        {
            return null;
        }

        // 아이템 ID 반환
        return _boundItem.ItemId;
    }

    // 선택 상태 함수
    public void ChangeSelectedState(bool isSelected)
    {
        if (_selectedImage != null)
        {
            _selectedImage.gameObject.SetActive(isSelected);
        }
    }

    // 버튼 클릭 이벤트 함수
    private void OnClickSelect()
    {
        _onSelectEvent?.Invoke(_boundItem);
    }
}
