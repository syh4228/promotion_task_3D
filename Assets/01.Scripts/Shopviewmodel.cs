using UnityEngine;
using System;
using System.Collections.Generic;

// 상점 아이템 뷰 모델
public class ShopItemViewModel
{
    public string ItemId;
    public string ItemName;
    public string IconPath;
    public int Price;
}

// 구매 시도 결과를 표현하는 상태값
public enum ShopPurchaseResult
{
    Success, // 성공
    NotEnoughGold, // 골드 부족
    ItemNotFound, // 존재하지 않는 아이템
    SystemNotReady // 필요한 시스템
}

// 상점 뷰 모델
// 실제 데이터 (GameData, CurrencyManager 등) ViewModel이 대신 접근
// UI에 전달
public class ShopViewModel
{
    // 상점 아이템 목록 변경되면 view에게 전달 하는 이벤트
    public event Action<List<ShopItemViewModel>> OnShopItemListChanged;
    // 현재 골드 변경 되면 view에게 전달 이벤트
    public event Action<int> OnGoldChanged;
    // 구매결과를  view에게 전달 이벤트
    public event Action<ShopPurchaseResult, string> OnPurchaseResult;

    // 상점에서 판매하는 아이템 ID 리스트로 저장
    private List<string> _shopItemIdList;

    // 판매할 아이템 ID 목록을 전달받는 함수
    public ShopViewModel(List<string> shopItemIdList)
    {
        _shopItemIdList = shopItemIdList;
    }

    // ViewModel 초기화 함수
    public void RequestInitialize()
    {
        if (CurrencyManager.Instance != null)
        {
            // 골드 변경 이벤트 구독
            CurrencyManager.Instance.OnGoldChanged += HandleGoldChanged;
        }

        RefreshGold(); // 현재 골드 UI 갱신
        RefreshShopItemList(); // 상점 목록 UI 생성
    }

    // ViewModel 제거 시 호출 함수
    public void Dispose()
    {
        if (CurrencyManager.Instance != null)
        {
            // 골드 변경 이벤트 구독 해제
            CurrencyManager.Instance.OnGoldChanged -= HandleGoldChanged;
        }
    }

    // 골드 변경 이벤트를 함수
    private void HandleGoldChanged(int newGold)
    {
        // 변경된 값을 전달
        OnGoldChanged?.Invoke(newGold);
    }

    // 현재 골드 UI를 갱신 함수
    private void RefreshGold()
    {
        if (CurrencyManager.Instance != null)
        {
            // 골드 변경 이벤트를 함수 호출
            HandleGoldChanged(CurrencyManager.Instance.Gold);
        }
    }

    // 판매 아이템 목록 함수
    private void RefreshShopItemList()
    {
        // UI에서 사용할 ViewModel 리스트
        List<ShopItemViewModel> resultList = new List<ShopItemViewModel>();

        // 판매 중인 모든 아이템 하나씩 확인
        foreach (string itemId in _shopItemIdList)
        {
            // GameData에서 실제 데이터 확인
            ItemData itemData = GetItemDataSafely(itemId);

            // 데이터 없으면
            if (itemData == null)
            {
                continue; // 건너뜀
            }

            // UI용 ViewModel 생성
            ShopItemViewModel itemViewModel = new ShopItemViewModel();
            itemViewModel.ItemId = itemData.Id;
            itemViewModel.ItemName = itemData.Name;
            itemViewModel.IconPath = itemData.IconPath;
            itemViewModel.Price = itemData.PurchasePrice;

            resultList.Add(itemViewModel);
        }

        // View에게 리스트 전달
        OnShopItemListChanged?.Invoke(resultList);
    }

    // 구매 요청 함수
    public void RequestPurchase(string itemId)
    {
        // 아이템 데이터 조회
        ItemData itemData = GetItemDataSafely(itemId);

        // 데이터 없으면
        if (itemData == null)
        {
            // 구매 결과 함수 호출
            NotifyPurchaseResult(ShopPurchaseResult.ItemNotFound, itemId);
            return;
        }

        // 재화 매니저가 없으면
        if (CurrencyManager.Instance == null)
        {
            // 구매 결과 함수 호출
            NotifyPurchaseResult(ShopPurchaseResult.SystemNotReady, itemData.Name);
            return;
        }

        // 구매 하기에 골드가 충분한지 검사
        bool hasEnoughGold = CurrencyManager.Instance.HasEnoughGold(itemData.PurchasePrice);

        if (hasEnoughGold == false) // 부족하면
        {
            NotifyPurchaseResult(ShopPurchaseResult.NotEnoughGold, itemData.Name);
            return;
        }

        // 구매 금액 골드 차감
        bool isSpendSuccess = CurrencyManager.Instance.SpendGold(itemData.PurchasePrice);

        if (isSpendSuccess == false) // 부족하면
        {
            // 조건 검사와 실제 차감 사이의 경합(동시 구매 등) 방어
            NotifyPurchaseResult(ShopPurchaseResult.NotEnoughGold, itemData.Name);
            return;
        }

        // 인벤토리 시스템에 아이템 지급
        if (GameManager.Inst != null && GameManager.Inst.Inventory != null)
        {
            GameManager.Inst.Inventory.AddItem(itemData.Id, 1);
        }
        else
        {
            Debug.LogWarning("[ShopViewModel] 인벤토리 시스템이 준비되지 않아 골드만 차감되고 아이템은 지급되지 않았습니다.");
        }

        NotifyPurchaseResult(ShopPurchaseResult.Success, itemData.Name);
    }

    // 인벤토리에 아이템 지급 함수
    private ItemData GetItemDataSafely(string itemId)
    {
        if (GameDataManager.Instance == null)
        {
            return null;
        }

        return GameDataManager.Instance.GetItemData(itemId);
    }

    // 구매 결과 함수
    private void NotifyPurchaseResult(ShopPurchaseResult result, string itemName)
    {
        OnPurchaseResult?.Invoke(result, itemName);
    }
}
