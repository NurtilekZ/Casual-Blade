using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    
    private Color currentColor;
    private int health = 100;

    public void Start()
    {
        currentColor = GetComponent<SpriteRenderer>().color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                GameManager.Instance.GameOver();
                break;
            case "Blade":
                TakeDamage();
                break;     
        }
    }

    private void TakeDamage()
    {
        animator.SetTrigger("Pop");
        GetComponent<SpriteRenderer>().color = Color.Lerp(currentColor, Color.yellow, Time.deltaTime);
        if (health == 0)
        {
            animator.SetTrigger("Explode");
        }
    }

    //Methods called in Animation Event
    private void DisableGameObject()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.SetActive(false);
    }
    
    private void DisableCollider()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
}