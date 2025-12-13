using UnityEngine;

public class enemy : MonoBehaviour
{
    GameObject ship;
    public float moveSpeed = 3f;
    public float stopDistance = 2.5f;

    void Start()
    {
        ship = GameManager.Instance.ship;    
    }

    void Update()
    {
        if (ship == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, ship.transform.position);

        if (distance > stopDistance)
        {
            MoveTowardsShip();
        }

    }
    void MoveTowardsShip()
    {
        Vector3 direction = (ship.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

}
