using System.Collections;
using Pooling;
using Pooling.Spawnables;
using UnityEngine;

public class BladesHolder : MonoBehaviour
{
    #region Singleton
    public static BladesHolder Instance;

    private void CreateSingleton()
    {
        Instance = this;
    }
    
    #endregion

    public static int CurrentBladesNumber;
    public float radius;
    public int boosterSpeed;
    public int bladeDamage = 20;

    private float timer;

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        SetupBlades(6);
    }

    public void SetupBlades(int number)
    {
        DisableCurrentBlades();
        
        CurrentBladesNumber = number;
        for (int i = 0; i < CurrentBladesNumber; i++)
        {
            float angle = i * Mathf.PI * 2 / CurrentBladesNumber;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * (
            CurrentBladesNumber / radius), Mathf.Sin(angle) * (CurrentBladesNumber / radius));
            GameObject blade = ObjectPooler.Instance.SpawnFromPool("Blade", transform.position - newPos, Quaternion.identity, transform);
            blade.GetComponent<Animator>().SetTrigger("Pick");
            Vector3 direction = transform.position - blade.transform.position;
            float bladeAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(bladeAngle, Vector3.forward);
            blade.transform.rotation = rotation;
            blade.transform.parent = transform;
        }
    }

    private void DisableCurrentBlades()
    {
        if (transform.childCount == 0) return;
        var blades = GetComponentsInChildren<Blade>();
        foreach (var blade in blades) 
        { 
            blade.gameObject.SetActive(false);
        }
    }

    public void ActivateSpeedBooster()
    {
        timer += 5;
        GetComponent<Animator>().speed = boosterSpeed;
        StartCoroutine(DeactivateBooster());
    }

    private IEnumerator DeactivateBooster()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            --timer;
        }
        GetComponent<Animator>().speed = 1;
    }
}