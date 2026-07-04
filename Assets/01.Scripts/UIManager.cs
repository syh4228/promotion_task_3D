using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글턴 선언

    [Header("UI 연결")]
    [SerializeField] private Text _hpText;     // HP 텍스트 표기
    [SerializeField] private Text _goldText; // 보유 골드 텍스트 표기

    [Header("인벤토리 창 연결")]
    [SerializeField] private GameObject _inventoryUIObject;

    [Header("상점 창 연결")]
    [SerializeField] private GameObject _shopUIObject;

    // PlayerState가 쏘는 이벤트 구독용
    [SerializeField] private PlayerState _PlayerState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (_inventoryUIObject != null)
        {
            _inventoryUIObject.SetActive(false);
        }

        if (_PlayerState == null)
        {
            _PlayerState = FindFirstObjectByType<PlayerState>();
        }

        if (_PlayerState != null)
        {
            // 플레이어 체력변경 구독
            _PlayerState.OnHpChanged += UpdateHpText;
            UpdateHpText(_PlayerState.CurrentHP, _PlayerState.MaxHp);
        }
        else
        {
            Debug.LogWarning("[UIManager] PlayerState를 찾을 수 없어 체력 UI를 구독 못했습니다.");
        }

        if (CurrencyManager.Instance != null)
        {
            // 골드 변경 이벤트 구독
            CurrencyManager.Instance.OnGoldChanged += UpdateGoldText;
            // 변경된 골드 값 받아오기
            UpdateGoldText(CurrencyManager.Instance.Gold);
        }
        else
        {
            Debug.LogWarning("[UIManager] CurrencyManager를 찾을 수 없어 골드 UI를 구독하지 못했습니다.");
        }
    }

    // 구독 해제
    private void OnDestroy()
    {
        if (_PlayerState != null)
        {
            _PlayerState.OnHpChanged -= UpdateHpText;
        }
    }

    // 체력 텍스트 표시 함수
    public void UpdateHpText(int currentHp, int maxHp) 
    {
        if (_hpText != null)
        {
            _hpText.text = "HP: " + currentHp + " / " + maxHp;
        }
    }

    // 골드 텍스트 표시 함수
    public void UpdateGoldText(int gold)
    {
        if (_goldText != null)
        {
            _goldText.text = "보유 골드: " + gold;
        }
    }

    public void ToggleInventoryUI()
    {
        if (_inventoryUIObject != null)
        {
            bool isOpen = _inventoryUIObject.activeSelf;
            _inventoryUIObject.SetActive(!isOpen);
        }
        else
        {
            Debug.LogWarning("[UIManager] 인벤토리 UI 오브젝트가 인스펙터에 연결되지 않았습니다!");
        }
    }

    public InventoryUI GetInventoryUI()
    {
        if (_inventoryUIObject != null)
        {
            return _inventoryUIObject.GetComponent<InventoryUI>();
        }
        return null;
    }

    // 상점 Npc 상호작용시 호출 함수
    public void ToggleTradeUI()
    {
        // UI가 열려있으면
        if (IsTradeUIOpen() == true)
        {
            CloseTradeUI(); // UI 닫기 호출
        }
        else // 아니면
        {
            OpenTradeUI(); // UI 열기 호출
        }
    }

    // UI 열기 함수
    public void OpenTradeUI()
    {
        if (_shopUIObject != null)
        {
            _shopUIObject.SetActive(true);
        }

        if (_inventoryUIObject != null)
        {
            _inventoryUIObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // UI 닫기 함수
    public void CloseTradeUI()
    {
        if (_shopUIObject != null)
        {
            _shopUIObject.SetActive(false);
        }

        if (_inventoryUIObject != null)
        {
            _inventoryUIObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // UI 열려있는지 확인 함수
    public bool IsTradeUIOpen()
    {
        return _shopUIObject != null && _shopUIObject.activeSelf;
    }
}
