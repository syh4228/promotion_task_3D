using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance; // 싱글턴 인스턴스

    public static BattleManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] private PlayerState _playerStat; // 플레이어 스탯 컴포넌트 연결

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    // 플레이어의 공격 계산 함수
    public void ExecutePlayerAttack(PlayerState player, SlimeState enemy)
    {
        // 플레이어가 없고, 적이 없으면 반환
        if (player == null || enemy == null) return;

        // 플레이어 대미지 가져와 저장
        int finalDamage = player.AttackPower;

        Debug.Log($"[배틀 매니저] 플레이어가 적을 공격! 최종 대미지: {finalDamage}");

        // 슬라임 대미지 전달 함수 호출
        enemy.ApplyDamage(finalDamage);
    }

    // 적이 공격 계산 함수
    public void ExecuteEnemyAttack(SlimeState enemy, PlayerState player)
    {
        // 적이 없고, 플레이어가 없으면 반환
        if (enemy == null || player == null) return;

        // 적의 공격력을 가져와 저장
        int finalDamage = enemy.AttackPower;

        Debug.Log($"[배틀 매니저] 슬라임이 플레이어를 공격! 최종 대미지: {finalDamage}");

        // 플레이어에 대미지 전달 함수 호출
        player.ApplyDamage(finalDamage);
    }

    // 함정 대미지 계산 함수
    public void ExecuteEnvironmentDamage(PlayerState player, int damageAmount)
    {
        if (player == null) return; // 플레이어가 있으면

        Debug.Log($"함정에 피해 {damageAmount} 데미지");

        // 대미지 저장
        int finalDamage = damageAmount;

        // 플레이어에게 대미지 전달 함수 호출
        player.ApplyDamage(finalDamage);
    }

    // 회복 계산 함수
    public void RequestPlayerHeal(int healAmount)
    {
        if (_playerStat != null) // 플레이어 스탯이 있으면
        {
            // 플레이어 스탯에 회복할 양 전달
            _playerStat.ApplyHeal(healAmount);
        }
    }
}
