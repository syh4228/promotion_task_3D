using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // 싱글턴 선언

    [Header("UI 연결")]
    [SerializeField] private Text _hpText;     // HP 텍스트 표기
    [SerializeField] private Text _potionText; // 포션 사용갯수 텍스트 표기

    private void Awake()
    {
        Instance = this;
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
}
