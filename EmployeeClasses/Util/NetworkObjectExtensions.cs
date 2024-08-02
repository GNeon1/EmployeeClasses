using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace EmployeeClasses.Util
{
    public static class NetworkObjectExtensions
    {
        public static IEnumerator WaitUntilSpawned(this NetworkObject networkObject)
        {
            yield return new WaitUntil(() => networkObject.IsSpawned);
        }

        static IEnumerator RunActionAfterSpawned(NetworkObject networkObject, Action action)
        {
            yield return networkObject.WaitUntilSpawned();
            action();
        }

        public static void OnSpawn(this NetworkObject networkObject, Action action)
        {
            networkObject.StartCoroutine(RunActionAfterSpawned(networkObject, action));
        }
    }
}
