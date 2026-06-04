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
    Transform PlayerTransform { get; }
    Transform ModelHolder { get; }
    bool HasJumpBuffered { get; }
    bool DodgePressed { get; }
    bool IsDodging { get; }
    bool IsInvulnerable { get; }

    void Move();
    void DodgeMove(Vector3 direction, float speed);
    Vector3 CurrentMoveDirection { get; }
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


    bool IsDrawingWeapon { get; }
    bool IsSheathingWeapon { get; }
    bool WeaponEquipped { get; }
    void SetDrawingWeapon(bool value);
    void SetSheathingWeapon(bool value);
    bool TogglePressed { get; }
    void SetCombatMode(CombatMode mode);
    void EquipSwordToBack();
    void EquipSwordToHand();
    void ConsumeToggle();
}
