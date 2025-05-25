using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Demonstrations;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAgent : Agent
{
    public float moveSpeed = 1f;
    public float jumpForce = 4f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.51f;
    public Transform sensorRoot;

    public Rigidbody rb;
    private GameObject[] allPoints;
    private float timeSinceStart = 0f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        allPoints = GameObject.FindGameObjectsWithTag("Point");
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart > 15f)
        {
            Debug.Log("超時，重新開始");
            AddReward(-0.5f);
            ScoreManager.Instance.ResetScore();
            EndEpisode();
        }
    }

    private void FixedUpdate()
    {
        sensorRoot.position = transform.position;
        sensorRoot.rotation = Quaternion.identity; // 始終保持水平
    }

    protected override void Awake()
    {
        Application.runInBackground = true;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("📹 開始新 Episode：目前是否正在錄 demo：" + GetComponent<DemonstrationRecorder>()?.Record);
        // 重設位置與速度
        rb.linearVelocity = Vector3.zero;
        transform.localPosition = new Vector3(-9f, 0.5f, -9f); // 可根據需要調整起始位置
        ResetAllPoints();
        timeSinceStart = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 觀察 Agent 的速度與是否在地面上
        sensor.AddObservation(rb.linearVelocity);
        sensor.AddObservation(IsGrounded() ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 moveInput = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
        float jumpInput = actions.ContinuousActions[2];

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        
        if (IsGrounded() && jumpInput > 0.5f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        AddReward(+0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;

        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.wKey.isPressed) moveY = 1f;
        if (Keyboard.current.sKey.isPressed) moveY = -1f;
        if (Keyboard.current.dKey.isPressed) moveX = 1f;
        if (Keyboard.current.aKey.isPressed) moveX = -1f;

        ca[0] = moveX;
        ca[1] = moveY;
        ca[2] = Keyboard.current.spaceKey.isPressed && IsGrounded() ? 1f : 0f;
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Hit wall, ending episode");
            AddReward(-1.0f);
            ScoreManager.Instance.ResetScore();
            EndEpisode();
        }
        else if (other.CompareTag("Point"))
        {
            AddReward(+1.0f);
            timeSinceStart = 0f;

            // 加給人類看的分數
            ScoreManager.Instance.AddLearningScore(1);

            // 隱藏得分物件
            other.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    private void ResetAllPoints()
    {
        foreach (GameObject point in allPoints)
        {
            point.SetActive(true);
        }
    }
}
