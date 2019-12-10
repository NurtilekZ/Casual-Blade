using Pooling.Spawnables;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currentGameState != GameManager.GameState.GAME) return;
        if (Input.touchCount == 0) return;
        
        float x = Input.GetTouch(0).deltaPosition.x * speed * Time.deltaTime;
        float y = Input.GetTouch(0).deltaPosition.y * speed * Time.deltaTime;
        Vector2 translate = new Vector2(x,y);
        transform.Translate(translate);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Heavy"))
        {
            GameManager.Instance.FinishLevel(GameManager.GameState.DEFEAT);
        }
        else if (other.CompareTag("Zone")) other.transform.parent.GetComponent<Enemy>().seePlayer = true;
    }
}
