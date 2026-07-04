using UnityEngine;
using System;

// 재화 관리 매니저
public class CurrencyManager : MonoBehaviour
{
     // 싱글턴 선언
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int _gold = 500;

    public event Action<int> OnGoldChanged; // 골드 변경 이벤트

    public int Gold
    {
        get { return _gold; }
    }

    private void Awake()
    {
        Instance = this;
    }

    // 구매 조건 검사용 함수
    public bool HasEnoughGold(int amount)
    {
        return _gold >= amount;
    }

    // 골드 소비(아이템 구매) 함수
    public bool SpendGold(int amount)
    {
        if (HasEnoughGold(amount) == false)
        {
            return false;
        }

        _gold = _gold - amount;
        NotifyGoldChanged();
        return true;
    }

    // 골드 획득(아이템 판매) 함수
    public void AddGold(int amount)
    {
        _gold = _gold + amount;
        NotifyGoldChanged();
    }

    // 골드 변경시 이벤트 알림 함수
    private void NotifyGoldChanged()
    {
        OnGoldChanged?.Invoke(_gold);
    }
}
