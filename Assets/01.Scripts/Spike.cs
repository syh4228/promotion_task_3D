using UnityEngine;
using UnityEngine.Playables;

public class Spike : MonoBehaviour
{
    [Header("피해 설정")]
    [SerializeField] private int _spikeDamage = 10;

    [Header("파티클 효과")]
    public GameObject HitParticle;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerState playerState = collision.gameObject.GetComponent<PlayerState>();

        if (playerState != null)
        {
            BattleManager manager = BattleManager.Instance;

            if (manager == null)
            {
                manager = Object.FindFirstObjectByType<BattleManager>();
            }

            if (HitParticle != null)
            {
                Instantiate(HitParticle, collision.contacts[0].point, Quaternion.identity);
            }

            if (manager != null)
            {
                manager.ExecuteEnvironmentDamage(playerState, _spikeDamage);
                Debug.Log("스파이크 충돌 성공: 함정 데미지 적용 완료");
            }
            else
            {
                Debug.LogError("스파이크: 배틀 매니저를 찾을 수 없습니다!");
            }
        }
    }
}
