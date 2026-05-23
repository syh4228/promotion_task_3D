using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 5f; // 이동 속도

    [Header("점프 및 물리")]
    [SerializeField] private float _jumpForce = 5f; // 점프 힘
    [SerializeField] private bool _isGrounded; // 지면 체크

    [Header("컴포넌트")]
    [SerializeField] private Rigidbody Rigidbody_Player; // 플레이어 리지드바디 연결
    [SerializeField] private Groundcheck GroundDetecter; // 지면체크 연결
    [SerializeField] private Animatorcontroller Animatorcontroller; // 애니메이션 컨트롤러
}
