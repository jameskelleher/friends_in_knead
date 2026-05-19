using System.Collections;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float launchDelay = 0.5f;
    [Range(0.0f, 1.0f)]
    public float launchRangeY = 0.5f;
    public float launchSpeedX = 5f;
    public GameObject projectile;

    void Start()
    {
        StartCoroutine(LaunchProjectile());
    }

    void Update()
    {

    }

    IEnumerator LaunchProjectile()
    {
        while (true)
        {
            float height = Camera.main.orthographicSize;
            float projY = Random.Range(-1 * height * launchRangeY, height * launchRangeY);
            Vector3 pos = new Vector3(transform.position.x, projY, transform.position.z);
            GameObject instance = Instantiate(projectile, pos, Quaternion.identity);
            float speed = transform.position.x < 0.0f ? launchSpeedX : -1 * launchSpeedX;
            instance.GetComponent<Rigidbody2D>().linearVelocityX = speed;

            yield return new WaitForSeconds(launchDelay);
        }
    }
}
