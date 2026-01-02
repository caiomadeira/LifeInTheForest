
using Il2CppSystem.Runtime.Remoting.Messaging;
using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
using Sons.Gameplay;
using Sons.Items;
using Sons.Items.Core;
using Sons.StatSystem;
using SonsSdk;
using System.Reflection;
using TheForest.Utils;
using UnityEngine;
using static CoopPlayerUpgrades;
using static Il2CppMono.Globalization.Unicode.SimpleCollator;
using static RedLoader.RLog;

namespace LifeInTheForest;
public class LITFKelvinWeaponSystem
{
    // --- robby --- 
    private VailActor robby;
    private Sons.Gameplay.MeleeWeapon kickWeapon;
    private KelvinConfig currentConfig;
    // --- weapons sys ---
    private GameObject _fakeWeaponInstance;
    private Transform _weaponAnchor;
    private bool _isWeaponAttached = false;
    private int weaponId;

    // --- gun combat variables ---
    private VailActor _currentTarget { get; set; }
    private float _lastTargetScanTime { get; set; }
    private float _nextFireTime { get; set; }
    private float _fireRate { get; set; } = 1.5f;
    private float _gunRange { get; set; } = 25f;
    private int _shootLayerMask = ~(1 << 2 | 1 << 4 | 1 << 5);

    public LITFKelvinWeaponSystem(VailActor robby, int weaponId, KelvinConfig currentConfig)
    {
        this.robby = robby;
        this.weaponId = weaponId;
        this.currentConfig = currentConfig;
        Init();
    }

    private void Init()
    {
        Transform rightHandBone = FindBone(this.robby.transform, "RightHand");
        if (rightHandBone != null)
        {
            _weaponAnchor = rightHandBone.Find("RightHandWeapon");
            if (_weaponAnchor != null)
            {
                _weaponAnchor.gameObject.SetActive(true);
                RLog.Msg("[ImprovedKelvin] Anchor 'RightHandWeapon' encontrado com sucesso.");
            }
            else RLog.Error("[ImprovedKelvin] RightHand encontrado, mas 'RightHandWeapon' não existe dentro dele.");
        }
        else RLog.Error("[ImprovedKelvin] Osso 'RightHand' não encontrado.");
    }

    public void LITFKelvinWeaponSystemUpdate()
    {
        RLog.Msg("[DEBUG] SEARCHING FOR TARGET AND TRY TO SHOOT");
        _currentTarget = GetNearestEnemy();
        if (_currentTarget != null)
        {
            RLog.Msg($"[DEBUG] TARGET FOUND: {_currentTarget.name}. TRYNG TO SHOOT...");
            ForceShootGun();
        }
        else RLog.Msg("[DEBUG] NONE ENEMY CLOSEST TO SHOOT.");
    }

    public void LITFKelvinWeaponSystemHandleCombatState(bool shouldHaveWeapon)
    {
        if (shouldHaveWeapon != _isWeaponAttached)
        {
            ToggleWeapon(shouldHaveWeapon);
            _isWeaponAttached = shouldHaveWeapon;
        }
    }

    private void AttachFakeWeapon()
    {
        if (_weaponAnchor == null) return;

        int id = weaponId;
        var itemData = ItemDatabaseManager.ItemById(weaponId);
        if (itemData != null && itemData.HeldPrefab != null)
        {
            _fakeWeaponInstance = UnityEngine.Object.Instantiate(itemData.HeldPrefab).gameObject;

            // Remove física e colisão da arma falsa
            foreach (var c in _fakeWeaponInstance.GetComponentsInChildren<Collider>()) c.enabled = false;
            foreach (var rb in _fakeWeaponInstance.GetComponentsInChildren<Rigidbody>()) UnityEngine.Object.Destroy(rb);

            _fakeWeaponInstance.transform.SetParent(_weaponAnchor, false);

            // Posicionamento Manual (Ajuste conforme necessário)
            _fakeWeaponInstance.transform.localPosition = new Vector3(0f, -0.0195f, 0.0724f);
            _fakeWeaponInstance.transform.localRotation = Quaternion.Euler(90, 90, 0);
            _fakeWeaponInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            Msg("[ImprovedKelvin] Shotgun visual anexada.");
        }
    }

    private void ToggleWeapon(bool enable)
    {
        if (enable)
        {
            if (_fakeWeaponInstance == null) AttachFakeWeapon();
            if (_fakeWeaponInstance != null) _fakeWeaponInstance.SetActive(true);
        }
        else
        {
            if (_fakeWeaponInstance != null) _fakeWeaponInstance.SetActive(false);
        }
    }


    private void ForceShootGun() => PerformRaycastShot();

    public void LateUpdate()
    {
        if (currentConfig.canUseWeapons && _isWeaponAttached)
        {
            VailActor potentialTarget = GetNearestEnemy();

            if (potentialTarget != null)
            {
                _currentTarget = potentialTarget;
                UpdateAimingLogic();
            }
        }
    }

    private void UpdateAimingLogic()
    {
        if (_currentTarget != null && _currentTarget.transform != null)
        {
            AimAtTarget();
            TryShootGun();
        }
    }

    private void TryShootGun()
    {
        if (Time.time >= _nextFireTime)
        {
            PerformRaycastShot();
            _nextFireTime = Time.time + _fireRate;
        }
    }

    private void AimAtTarget()
    {
        Vector3 targetPos = _currentTarget.transform.position;
        targetPos.y = robby.transform.position.y;

        Vector3 direction = targetPos - robby.transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            robby.transform.rotation = Quaternion.Slerp(robby.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    private void PerformRaycastShot()
    {
        if (_fakeWeaponInstance == null)
        {
            Error("[Combat] ERRO: Arma Falsa é NULL. Recriando...");
            AttachFakeWeapon();
            return;
        }

        if (_currentTarget == null) return;

        Vector3 origin = _fakeWeaponInstance.transform.position + (_fakeWeaponInstance.transform.forward * 0.5f);
        Vector3 targetCenter = _currentTarget.transform.position + (Vector3.up * 1.2f);
        Vector3 direction = (targetCenter - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, _gunRange, _shootLayerMask))
        {
            var hitActor = hit.collider.GetComponentInParent<VailActor>();

            if (hitActor != null && hitActor != robby)
            {
                if (hitActor._statsManager != null && hitActor._statsManager._statsManager != null)
                {
                    var healthStat = hitActor._statsManager._statsManager.Stats[2]; // Health
                    if (healthStat != null)
                    {
                        float damage = 40f;
                        healthStat._currentValue -= damage;
                        if (healthStat._currentValue <= 0 && !hitActor.IsDead())
                        {
                            hitActor.SetDead(true);
                            Msg($"[KILL] {hitActor.name} eliminado.");
                        }
                    }
                }
            }
            else
            {
                RLog.Msg($"[MISS] Atingiu: {hit.collider.name} na Layer {hit.collider.gameObject.layer}");
            }
        }
    }

    private VailActor GetNearestEnemy()
    {
        if (VailWorldSimulation._instance == null) return null;

        float shortestDistance = _gunRange; 
        VailActor nearest = null;
        Vector3 myPos = robby.transform.position;

        foreach (var actor in VailWorldSimulation._instance._actors)
        {
            if (actor == null || actor.GetVailActor().IsDead() || actor.GetVailActor() == robby) continue;
            if (actor.GetVailActor().TypeId == VailActorTypeId.Carl || actor.GetVailActor().TypeId == VailActorTypeId.Billy)
            {
                float dist = Vector3.Distance(myPos, actor.GetVailActor().transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearest = actor.GetVailActor();
                }
            }
        }
        return nearest;
    }

    private Transform FindBone(Transform current, string name)
    {
        if (current.name == name) return current;
        for (int i = 0; i < current.childCount; ++i)
        {
            var found = FindBone(current.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }
}
