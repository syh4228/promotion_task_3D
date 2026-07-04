using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShopNpc : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleTradeUI();
        }
        else
        {
            Debug.LogWarning("[ShopNpc] UIManager 인스턴스를 찾을 수 없습니다.");
        }
    }
}
