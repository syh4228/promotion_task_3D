using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Text _goldText; // 보유 골드 표시
    [SerializeField] private Text _resultMessageText; // 구매 결과 출력

    [Header("선택 정보 / 구매 확정")]
    [SerializeField] private Text _selectedItemInfoText; // 아이템 정보 출력
    [SerializeField] private Button _confirmPurchaseButton; // 구매 확정 버튼

    [Header("슬롯 연결")]
    [SerializeField] private Transform _slotParent; // 생성될 슬롯의 부모
    [SerializeField] private GameObject _slotPrefab; // 슬롯 프리팹
    [SerializeField] private Button _exitButton; // 닫기 버튼

    [Header("판매 아이템 목록 (Id)")]
    [SerializeField] private List<string> _shopItemIdList = new List<string>();

    private ShopViewModel _viewModel; 
    // 슬롯을 리스트로 저장
    private List<ShopSlotUI> _slotPool = new List<ShopSlotUI>();
    private string _currentSelectedItemId; // 현재 선택된 아이템 ID

    private void OnEnable()
    {
        // 뷰모델 저장
        _viewModel = new ShopViewModel(_shopItemIdList);

        // 이벤트 연결
        // 아이템 목록 변경
        _viewModel.OnShopItemListChanged += HandleShopItemListChanged;
        _viewModel.OnGoldChanged += HandleGoldChanged; // 골드 변경
        _viewModel.OnPurchaseResult += HandlePurchaseResult; // 구매 결과

        if (_confirmPurchaseButton != null)
        {
            // 중복 방지
            _confirmPurchaseButton.onClick.RemoveAllListeners();
            // 클릭이벤트 등록
            _confirmPurchaseButton.onClick.AddListener(OnClickConfirmPurchase);
        }

        if (_exitButton != null)
        {
            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(OnClickExit);
        }

        ActivateConfirmButton(false); // 버튼 비활성화

        _viewModel.RequestInitialize(); // 초기화
    }

    private void OnDisable()
    {
        if (_viewModel != null)
        {
            // 구독 혜지
            _viewModel.OnShopItemListChanged -= HandleShopItemListChanged;
            _viewModel.OnGoldChanged -= HandleGoldChanged;
            _viewModel.OnPurchaseResult -= HandlePurchaseResult;

            _viewModel.Dispose(); // 내부 구독 정리 함수 호출
            _viewModel = null; // 뷰모델 정리
        }
    }

    // 골드 변경 함수
    private void HandleGoldChanged(int gold)
    {
        if (_goldText != null)
        {
            _goldText.text = "보유 골드: " + gold;
        }
    }

    // 아이템 리스트 전달 함수
    private void HandleShopItemListChanged(List<ShopItemViewModel> itemList)
    {
        int index = 0; // 슬롯 풀 인덱스로 저장

        // 진열할 아이템 개수만큼 반복
        for (index = 0; index < itemList.Count; index++)
        {
            // 인덱스 번호에 맞는 슬롯 저장
            ShopSlotUI slot = GetOrCreateSlot(index);
            slot.gameObject.SetActive(true); // 슬롯 활성화
            // 슬롯 실제 데이터 채우기
            slot.Bind(itemList[index], HandleSlotSelected);
        }

        // 아이템 개수보다 슬롯이 많이 남으면
        for (; index < _slotPool.Count; index++)
        {
            // 파괴하지 않고 비활성화
            _slotPool[index].gameObject.SetActive(false);
        }
    }

    // 슬롯 생성 함수
    private ShopSlotUI GetOrCreateSlot(int index)
    {
        // 인덱스가 슬롯 풀 범위 안에 있으면
        if (index < _slotPool.Count) 
        {
            // 새로 만들지 않고, 기존 슬롯 반환
            return _slotPool[index];
        }

        // 범위 안에 없으면 프리팹 복제해서 슬롯 오브젝트 저장
        GameObject slotObject = Instantiate(_slotPrefab, _slotParent);
        // 생성된 슬롯 부모슬롯UI의 자식으로 저장
        ShopSlotUI slotComponent = slotObject.GetComponent<ShopSlotUI>();
        _slotPool.Add(slotComponent); // 풀 리스트에 추가
        return slotComponent; // 슬롯 반환
    }

    // 슬롯 선택 함수
    private void HandleSlotSelected(ShopItemViewModel selectedItem)
    {
        // 선택된 아이템 저장
        _currentSelectedItemId = selectedItem.ItemId;

        // 슬롯 풀에 있는 슬롯 하나씩 꺼내서 확인
        foreach (ShopSlotUI slot in _slotPool)
        {
            // 선택한 슬롯이 활성화 되있고, 방금 선택된 아이템과 같은 아이템을 표시 중이면 저장
            bool isSelected = slot.gameObject.activeSelf && slot.GetItemId() == selectedItem.ItemId;
            slot.ChangeSelectedState(isSelected); // 선택된 슬롯 선택 상태 지정
        }

        // 선택 정보 텍스트가 있으면
        if (_selectedItemInfoText != null)
        {
            // 아이템 정보 저장
            _selectedItemInfoText.text = selectedItem.ItemName + " (" + selectedItem.Price + " G)";
        }

        ActivateConfirmButton(true); // 아이템 구매 버튼 활성화
    }

    // 구매 버튼 클릭 함수
    private void OnClickConfirmPurchase()
    {
        // 선택된 아이템이 없으면
        if (string.IsNullOrEmpty(_currentSelectedItemId) == true)
        {
            return; // 반환
        }

        if (_viewModel != null) // 뷰 모델이 있으면
        {
            // 실제 구매 처리 요청
            _viewModel.RequestPurchase(_currentSelectedItemId);
        }
    }

    // 닫기 버튼 함수
    private void OnClickExit()
    {
        if (UIManager.Instance != null)
        {
            // 상점UI 닫을때 인벤토리도 함께 닫기
            UIManager.Instance.CloseTradeUI();
        }
        else
        {
            // UIManager를 못 찾는 예외 상황이면 최소한 상점 패널만이라도 끈다
            gameObject.SetActive(false);
        }
    }

    // 구매 확정 버튼 관리 함수
    private void ActivateConfirmButton(bool isActive)
    {
        // 버튼이 있으면
        if (_confirmPurchaseButton != null)
        {
            // 버튼이 true면 활성화, false 면 비활성화
            _confirmPurchaseButton.gameObject.SetActive(isActive);
        }
    }

    // 이벤트 발생 호출 함수
    private void HandlePurchaseResult(ShopPurchaseResult result, string itemName)
    {
        // 결과 메시지 텍스트가 있으면
        if (_resultMessageText != null)
        {
            // 구매가 성공하면
            if (result == ShopPurchaseResult.Success)
            {
                _resultMessageText.text = itemName + " 구매 완료!";
            } // 실패하면
            else if (result == ShopPurchaseResult.NotEnoughGold)
            {
                _resultMessageText.text = "골드가 부족합니다.";
            }
            else if (result == ShopPurchaseResult.ItemNotFound) // 아이템이 없으면
            {
                _resultMessageText.text = "존재하지 않는 아이템입니다.";
            }
            else // 그외의 상황이 발생시
            {
                _resultMessageText.text = "상점 시스템이 아직 준비되지 않았습니다.";
            }
        }

        // 구매 성공 시 선택 상태를 해제한다 (인벤토리 UX와 동일)
        if (result == ShopPurchaseResult.Success)
        {
            // 선택 정보 초기화
            _currentSelectedItemId = null;
            ActivateConfirmButton(false); // 확정 버튼 비활성화

            // 모든 슬롯을 하나씩 확인
            foreach (ShopSlotUI slot in _slotPool)
            {
                // 채크 상태 비활성화
                slot.ChangeSelectedState(false);
            }
        }
    }
}
