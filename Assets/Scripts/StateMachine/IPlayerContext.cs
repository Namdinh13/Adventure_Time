using UnityEngine;

public interface IPlayerContext
{
    bool IsGrounded { get; }
    bool IsJumping { get; }
    float VerticalVelocity { get; }
    bool IsAttacking { get; }
    bool AttackPressed { get; }
    int ComboStep { get; }
    bool LockedOn { get; }
    bool IsHit { get; }
    CombatMode CombatMode { get; }
    Transform CurrentTarget { get; }
    bool HasJumpBuffered { get; }
    bool DodgePressed { get; }
    bool IsDodging { get; }
    bool IsInvulnerable { get; }

    void Move();
    void DodgeMove(Vector3 direction, float speed);
    void ApplyGravity();
    void ApplyJump();
    void ResetJumpState();
    void ConsumeJumpBuffer();
    void ConsumeDodge();
    Vector3 GetDodgeDirection();
    void SetInvulnerable(bool value);
    void SetDodging(bool value);

    void StartAttack();
    void StopAttack();
    void ConsumeAttack();
    void NextCombo();
    void ResetCombo();

    void StartHit();
    void StopHit();
}
