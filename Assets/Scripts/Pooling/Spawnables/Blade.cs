using UnityEngine;

namespace Pooling.Spawnables
{
    public class Blade : MonoBehaviour, IPooledObject
    {
        public void OnObjectSpawn()
         {
             if (transform.parent == ObjectPooler.Instance.gameObject.transform)
             {
                 tag = "Drop";
             }
             else if (transform.parent == BladesHolder.Instance.gameObject.transform)
             {
                 tag = "Blade";
             }
         }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CompareTag("Drop")) return;
            if (other.CompareTag("Enemy") || other.CompareTag("Heavy"))
            {
                other.GetComponent<Enemy>().TakeDamage();
            }
            else if (other.CompareTag("Drop"))
            {
                if (transform.parent == BladesHolder.Instance.gameObject.transform)
                {
                    other.gameObject.SetActive(false);
                    BladesHolder.Instance.SetupBlades(BladesHolder.CurrentBladesNumber + 1);
                }
            }
            else if (other.CompareTag("Booster"))
            {
                other.gameObject.SetActive(false);
                BladesHolder.Instance.ActivateSpeedBooster();
            }
        }
    }
}
