using UnityEngine;

public interface IPlayerContext
{
    void Move();
    void DodgeMove(Vector3 direction, float speed);
    void HandleLockRotation();
    void ApplyGravity();
    void ApplyJump();
    void ResetJumpState();
    void ConsumeJumpBuffer();
    void ConsumeDodge();
    void SetInvulnerable(bool value);
    void SetDodging(bool value);
    void ResetModelRotation();
    void StartAttack();
    void StopAttack();
    void ConsumeAttack();
    void NextCombo();
    void ResetCombo();
    void StartHit();
    void StopHit();
    void SetDrawingWeapon(bool value);
    void SetSheathingWeapon(bool value);
    void SetCombatMode(CombatMode mode);
    void EquipSwordToBack();
    void EquipSwordToHand();
    void ConsumeToggle();
    void StartDeath();
    void TriggerGameOver();

    int ComboStep { get; }

    float VerticalVelocity { get; }

    bool IsGrounded { get; }
    bool IsJumping { get; }
    bool IsAttacking { get; }
    bool AttackPressed { get; }
    bool LockedOn { get; }
    bool IsHit { get; }
    bool IsDrawingWeapon { get; }
    bool IsSheathingWeapon { get; }
    bool WeaponEquipped { get; }
    bool HasJumpBuffered { get; }
    bool DodgePressed { get; }
    bool IsDodging { get; }
    bool IsInvulnerable { get; }
    bool TogglePressed { get; }
    bool IsDead { get; }
    bool IsAttackHeld { get; }

    CombatMode CombatMode { get; }
    Transform CurrentTarget { get; }
    Transform PlayerTransform { get; }
    Transform ModelHolder { get; }
    Vector3 CurrentMoveDirection { get; }

   
}
