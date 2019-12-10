using System.Collections;
using UnityEngine;

namespace Pooling.Spawnables
{
    public class Booster : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(DeactivateObject());
        }

        private IEnumerator DeactivateObject()
        {
            yield return new WaitForSeconds(15f);
            gameObject.SetActive(false);
        }
    }
}
