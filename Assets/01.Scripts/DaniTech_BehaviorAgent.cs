using UnityEngine;
using Unity.Behavior;
using NUnit.Framework;
using System.Collections.Generic;

public class DaniTech_BehaviorAgent : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent BehaviorAgent_Self;
    [SerializeField] private List<GameObject> PatrolSpotGameObjectList;

    private void OnEnable()
    {
        if (PatrolSpotGameObjectList != null && PatrolSpotGameObjectList.Count > 0)
        {
            BehaviorAgent_Self.SetVariableValue("PatrolSpotList", PatrolSpotGameObjectList);
        }
    }

}
