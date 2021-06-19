using UnityEngine;

public class Target : MonoBehaviour
{
    public int type;    // 1 = Melle Enemy
                        // 2 = Ranged Enemy
    EnemyController EnemyC;
    EnemyControllerRanged EnemyCRanged;
    [SerializeField] int DamageMultiplier;

    private void Awake()
    {
        switch (type)
        {
            case 1:
                {
                    EnemyC = GetComponentInParent<EnemyController>();
                    break;
                }
            case 2:
                {
                    EnemyCRanged = GetComponent<EnemyControllerRanged>();
                    break;
                }
        }
    }
    public void TakeDamage(float amount)
    {
        amount *= DamageMultiplier;
        switch (type)
        {
            case 1:
                {
                    EnemyC.TakeEnemyDamage(amount);
                    break;
                }
            case 2:
                {
                    EnemyCRanged.TakeEnemyDamage(amount);
                    break;
                }
        }
    }


}
