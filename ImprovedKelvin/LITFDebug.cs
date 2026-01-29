
using RedLoader;
using Sons.Ai.Vail;

namespace LifeInTheForest;
public class LITFDebug
{
    public static bool isDebugActive = true;

    public static void KelvinDumpConfig(KelvinConfig currentConfig, bool agingConfigDone)
    {
        if (isDebugActive)
        {
            try
            {
                if (agingConfigDone)
                {
                    RLog.Msg("------------------ Kelvin Config ------------------");
                    RLog.Msg($"stageName ={currentConfig.stageName}");
                    RLog.Msg($"minYear ={currentConfig.startingThreshold}");
                    RLog.Msg($"headTexture ={currentConfig.headTexture}");
                    RLog.Msg($"tuxedoTexture ={currentConfig.tuxedoTexture}");
                    RLog.Msg($"hoddieTexture ={currentConfig.hoddieTexture}");
                    RLog.Msg($"hasBeard ={currentConfig.hasBeard}");
                    RLog.Msg($"hasHair ={currentConfig.hasHair}");
                    RLog.Msg($"armorType ={currentConfig.armorType.ToString()}");
                    RLog.Msg($"maxHealth ={currentConfig.maxHealth}");
                    RLog.Msg($"maxStamina ={currentConfig.maxStamina}");
                    RLog.Msg($"maxHeartRateStat ={currentConfig.maxHeartRateStat}");
                    RLog.Msg($"maxFullness ={currentConfig.maxFullness}");
                    RLog.Msg($"maxFear ={currentConfig.maxFear}");
                    RLog.Msg($"maxAnger ={currentConfig.maxAnger}");
                    RLog.Msg($"moveSpeedMultiplier ={currentConfig.moveSpeedMultiplier}");
                    RLog.Msg($"meleeWeaponDamage ={currentConfig.meleeWeaponDamage}");
                    RLog.Msg($"canCombat ={currentConfig.canCombat}");
                    RLog.Msg($"canUseWeapons ={currentConfig.canUseWeapons}");
                    RLog.Msg($"canUseCosmeticArmor ={currentConfig.canUseCosmeticArmor}");
                    RLog.Msg($"canRevive ={currentConfig.canRevive}");
                    RLog.Msg($"removeBehaviors ={currentConfig.removeBehaviors}");
                    RLog.Msg($"IsAbleToUseGoldenArmor ={currentConfig.IsAbleToUseGoldenArmor}");
                    RLog.Msg($"onlyKilledByPlayer ={currentConfig.onlyKilledByPlayer}");
                    RLog.Msg($"shotgunKnockbackTime ={currentConfig.shotgunKnockbackTime}");
                    RLog.Msg("------------------ Kelvin Config ------------------");
                }
                else { RLog.Msg("[Improved Kelvin] Can't dump aging config info. Isn't initialized. Too soon."); }
            }
            catch (System.Exception e) { RLog.Error($"[KelvinDumpConfig] Error: {e}"); }
        }
    }

    public static void KelvinDumpInfo(VailActor _actor)
    {
        if (!isDebugActive || _actor == null) return;
        RLog.Msg($"ROBBY STAT LIST {_actor._staggeredAmount}");

        for (int i = 0; i < _actor._statsManager._statsManager.Stats.Count; i++)
        {
            RLog.Msg($"State name {i}: {_actor._statsManager._statsManager.Stats[i].GetName()}");
        }

        try
        {
            if (_actor._vailGiftManager != null)
                RLog.Msg($"_vailGiftManager.enabled: {_actor._vailGiftManager.enabled}");
            else
                RLog.Msg("_vailGiftManager: NULL");
            if (_actor.GetWorldSimController() != null)
                RLog.Msg($"GetWorldSimController().name: {_actor.GetWorldSimController().name}");
            else RLog.Msg("GetWorldSimController(): NULL");

            if (_actor._worldSimController != null)
                for (int i = 0; i < _actor.GetWorldSimController()._actions.Count; i++)
                {
                    RLog.Msg($"robby.GetWorldSimController()._actions[i].GetName() {i}: {_actor.GetWorldSimController()._actions[i].GetName()}");
                }

            RLog.Msg($"AnimStateParameters: {_actor.AnimStateParameters}");
            if (_actor._animator != null)
                RLog.Msg($"robby._animator.name: {_actor._animator.name}");
            else
                RLog.Msg("_animator: NULL");
            RLog.Msg($"robby._animEvents.name: {_actor._animEvents.name}");
            for (int i = 0; i < _actor._stateAnimatorSettings.Count; i++)
            {
                RLog.Msg($"robby._stateAnimatorSettings._items[{i}]._state._name: {_actor._stateAnimatorSettings._items[i]._state._name}");
            }
            RLog.Msg($"robby.CheckPlayerSentimentLevel(0): {_actor.CheckPlayerSentimentLevel(0)}");
            if (_actor.ActorStimuli != null)
                RLog.Msg($"robby.ActorStimuli.name: {_actor.ActorStimuli.name}");
            if (_actor._stimuliGroups != null)
                RLog.Msg($"robby._stimuliGroups.name: {_actor._stimuliGroups.name}");
            RLog.Msg($"robby.GetInventoryItemType(): {_actor.GetInventoryItemType()}");
            RLog.Msg($"robby._hasInventory: {_actor._hasInventory}");

            if (_actor._inventoryManager != null)
            {
                for (int i = 0; i < _actor._inventoryManager._carryAttachments.Count; i++)
                {
                    RLog.Msg($"robby._inventoryManager._carryAttachments {i}: {_actor._inventoryManager._carryAttachments[i].name}");
                }
            }
            RLog.Msg($"speedFloatParam={_actor._speedFrame}");
            RLog.Msg($"_staggeredAmount: {_actor._staggeredAmount}");
            RLog.Msg($"_staggerLevel: {_actor._staggerLevel}");
            RLog.Msg($"_staggeredAmount: {_actor._staggerPerHit}");
            RLog.Msg($"_staggeredAmount: {_actor._staggeredAmount}");

            RLog.Msg($"_isFemale: {_actor._isFemale}");
            RLog.Msg($"_helicopterHeld: {_actor._helicopterHeld}");

            if (_actor._healthSettings != null)
            {
                RLog.Msg($"_poisonFearRate: {_actor._healthSettings._poisonFearRate}");
                RLog.Msg($"_poisonHealthRate: {_actor._healthSettings._poisonHealthRate}");
                RLog.Msg($"_poisonEnergyRate: {_actor._healthSettings._poisonEnergyRate}");
                RLog.Msg($"_dyingHealthRecover: {_actor._healthSettings._dyingHealthRecover}");
                RLog.Msg($"_health: {_actor._healthSettings._health}");
                RLog.Msg($"_onlyKilledByPlayer: {_actor._healthSettings._onlyKilledByPlayer}");

            }
            else RLog.Msg("_healthSettings: NULL");

            RLog.Msg($"_dropEquipItemsOnDeath: {_actor._dropEquipItemsOnDeath}");

            if (_actor._dismembermentController != null)
                RLog.Msg($"_dismembermentController: {_actor._dismembermentController.name}");
            else
                RLog.Msg("_dismembermentController: NULL");

            RLog.Msg($"_dismemberedParts Count: {_actor._dismemberedParts}");
            RLog.Msg($"_damageEnabled: {_actor._damageEnabled}");

            if (_actor._damageController != null)
                RLog.Msg($"GetBlockDamageMultiplier: {_actor._damageController.GetBlockDamageMultiplier()}");
            else
                RLog.Msg("_damageController: NULL");

            RLog.Msg($"_shotgunKnockbackTime: {_actor._shotgunKnockbackTime}");

            if (_actor._meleeWeapons != null)
            {
                RLog.Msg($"Melee Weapons Count: {_actor._meleeWeapons.Count}");
                for (int i = 0; i < _actor._meleeWeapons.Count; i++)
                {
                    if (_actor._meleeWeapons[i] != null)
                        RLog.Msg($"robby melee weapon {i}: ID={_actor._meleeWeapons[i]._id}");
                    else
                        RLog.Msg($"robby melee weapon {i}: ITEM NULL");
                }
            }
            else RLog.Msg("_meleeWeapons List: NULL");

            if (_actor._statsManager != null && _actor._statsManager._statsManager != null)
            {
                var statsList = _actor._statsManager._statsManager.Stats;
                if (statsList != null)
                {
                    for (int i = 0; i < statsList.Count; i++)
                    {
                        if (statsList[i] != null)
                            RLog.Msg($"State name {i}: {statsList[i].GetName()} Value: {statsList[i]._currentValue}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            RLog.Error($"[DEBUG F5] Erro ao printar stats: {e.Message}");
        }

        RLog.Msg("=== [DEBUG F5] Fim do Dump ===");
    }
}

