using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Rigidbody player;
    public Vector3 offset;
    public Vector3 startPose = new Vector3(-9.0f, 2.89f, -12.59f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position - offset;
    }
}
