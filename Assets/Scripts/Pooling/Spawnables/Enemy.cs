using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pooling.Spawnables
{
    public class Enemy : MonoBehaviour, IPooledObject
    {
        public Animator animator;
        public int health;
        public float speed;
        public bool seePlayer;

        private Transform player;
        private TextMeshProUGUI healthText;
        private bool isDead;
        private Vector3 initialScale;

        public void OnObjectSpawn()
        {
            if (CompareTag("Heavy"))
            {
                player = FindObjectOfType<PlayerController>().gameObject.transform;
            }

            isDead = false;
            GetComponent<CircleCollider2D>().enabled = true;
            transform.localScale = new Vector3(0.3f,0.3f,0.3f);
            health = Random.Range(5, 20 * GameManager.Instance.level);
            healthText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            healthText.text = health.ToString();
            StartCoroutine(EnableScreenBoundaries());
        }

        private IEnumerator EnableScreenBoundaries()
        {
            yield return new WaitUntil(() => ObjectIsInsideScreenBoundaries());
            GetComponent<StayInsideTheScreen>().enabled = true;
        }

        private bool ObjectIsInsideScreenBoundaries()
        {
            var position = transform.position;
            return position.x > Camera.main.transform.position.x - 2f 
                   && position.x < Camera.main.transform.position.x + 2f
                   && position.y > Camera.main.transform.position.y - 4f
                   && position.y < Camera.main.transform.position.y + 4f;
        }

        private void Update()
        {
            if (GameManager.Instance.currentGameState != GameManager.GameState.GAME) return;
            if (isDead) return;
            if (seePlayer)
            {
                Vector3 lookDirection = player.position - transform.position;
                transform.up = Vector2.Lerp(transform.up, lookDirection, Time.deltaTime);
            }

            transform.Translate(speed * Time.deltaTime * Vector2.up);
        }

        public void TakeDamage()
        {
            if (GameManager.Instance.currentGameState != GameManager.GameState.GAME) return;
            health -= BladesHolder.Instance.bladeDamage;
            if (health <= 0)
            {
                GetComponent<CircleCollider2D>().enabled = false;
                animator.SetTrigger("Explode");
                healthText.text = 0.ToString();
                isDead = true;
                return;
            }
            healthText.text = health.ToString();
            animator.SetTrigger("Damage");
        }

        //Method called in Animation Event
        private void DisableGameObject()
        {
            if (gameObject.CompareTag("Heavy"))
                switch (Random.Range(0, 3))
                {
                    case 0 :
                        break;
                    case 1 :
                        if (BladesHolder.CurrentBladesNumber < 15)
                            ObjectPooler.Instance.SpawnFromPool("Blade", transform.position, Quaternion.identity, ObjectPooler.Instance.transform);
                        break;
                    case 2 :
                        ObjectPooler.Instance.SpawnFromPool("Booster", transform.position, Quaternion.identity, ObjectPooler.Instance.transform);
                        break;
                }
            transform.position = Vector3.left;
            transform.parent = ObjectPooler.Instance.transform;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GetComponent<StayInsideTheScreen>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            seePlayer = false;
        }
    }
}