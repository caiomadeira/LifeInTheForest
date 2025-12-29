using System.Collections.Generic;
using System.Linq;
using RedLoader;
using Sons.Ai.Vail;
using SonsSdk;
using UnityEngine;

namespace LifeInTheForest;

[RegisterTypeInIl2Cpp]
public class TattooAttachment : MonoBehaviour
{
    public static List<TattooAttachment> Instances = new List<TattooAttachment>();

    public static MelonEvent<VailActor> OnActorAdded = new MelonEvent<VailActor>();

    private VailActor _actor;

    private VailActorTypeId _actorType;

    private void Start()
    {
        Instances.Add(this);
        _actor = GetComponent<VailActor>();
        _actorType = _actor.TypeId;
        OnActorAdded.Invoke(_actor);
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }

    public static IEnumerable<VailActor> GetActors(VailActorTypeId actorType)
    {
        return from x in Instances
               where x._actorType == actorType
               select x._actor;
    }

    public static void Attach(VailActorTypeId actorType)
    {
        ActorTools.GetPrefab(actorType).gameObject.AddComponent<TattooAttachment>();
    }
}