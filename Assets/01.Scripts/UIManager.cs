using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글턴 선언

    [Header("UI 연결")]
    [SerializeField] private Text _hpText;     // HP 텍스트 표기
    [SerializeField] private Text _potionText; // 포션 사용갯수 텍스트 표기

    [Header("인벤토리 창 연결")]
    [SerializeField] private GameObject _inventoryUIObject;

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
    }

    // 체력 텍스트 표시 함수
    public void UpdateHpText(int currentHp, int maxHp) 
    {
        if (_hpText != null)
        {
            _hpText.text = "HP: " + currentHp + " / " + maxHp;
        }
    }

    // 포션 텍스트 표시 함수
    public void UpdatePotionText(int potionCount)
    {
        if (_potionText != null)
        {
            _potionText.text = "사용갯수: " + potionCount;
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
}
