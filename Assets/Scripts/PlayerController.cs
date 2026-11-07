using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public ItemSpawner spawner;
    public PlayerHPBar hpBar;
    public float[] laneX = { -2f, 0f, 2f };
    public float laneEaseDuration = 0.2f;
    private int currentLane = 1;
    private float laneStartX;
    private float laneTargetX;
    private float laneElapsed = 1f;
    public float groundY = -1f;
    public float jumpPower = 8f;
    public float gravity = 16f;
    private float verticalStep;
    private bool isJumping;
    public Vector3 playerPos = new Vector3(0f, -1f, 0f);
    public float zHitRange = 15f;
    public float hitRadius = 1f;
    public float hitDamage = 5f;

    private void Start()
    {
        playerPos = new Vector3(laneX[currentLane], groundY, 0f);
        laneStartX = playerPos.x;
        laneTargetX = playerPos.x;
    }

    private void Update()
    {
        HandleInput();
        UpdateLanePosition();
        UpdateJump();
        ApplyPerspective();
        CheckObstacleHits();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            ChangeLane(-1);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            ChangeLane(1);
        }

        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && !isJumping)
        {
            verticalStep = jumpPower;
            isJumping = true;
        }
    }

    private void ChangeLane(int direction)
    {
        int nextLane = Mathf.Clamp(currentLane + direction, 0, laneX.Length - 1);
        if (nextLane == currentLane) return;

        currentLane = nextLane;
        laneStartX = playerPos.x;
        laneTargetX = laneX[currentLane];
        laneElapsed = 0f;
    }

    private void UpdateLanePosition()
    {
        if (laneElapsed < laneEaseDuration)
        {
            laneElapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(laneElapsed / laneEaseDuration);
            float easedT = Ease.EaseOutQuad(rawT);
            playerPos.x = Mathf.Lerp(laneStartX, laneTargetX, easedT);
        }
        else
        {
            playerPos.x = laneTargetX;
        }
    }

    private void UpdateJump()
    {
        if (!isJumping) return;

        playerPos.y += verticalStep * Time.deltaTime;
        verticalStep -= gravity * Time.deltaTime;

        if (playerPos.y <= groundY)
        {
            playerPos.y = groundY;
            verticalStep = 0f;
            isJumping = false;
        }
    }

    private void ApplyPerspective()
    {
        float perspective = CameraComponent.focalLenth / (CameraComponent.focalLenth + playerPos.z);
        transform.localScale = Vector3.one * perspective;
        transform.position = new Vector2(playerPos.x, playerPos.y) * perspective;
    }

    private void CheckObstacleHits()
    {
        if (spawner == null) return;

        for (int i = 0; i < spawner.activeItems.Count; i++)
        {
            Item item = spawner.activeItems[i];
            if (item == null || item.hasHit) continue;

            float zDiff = Mathf.Abs(item.itemPosition.z - playerPos.z);
            if (zDiff <= zHitRange)
            {
                Vector2 toObstacle = new Vector2(item.itemPosition.x - playerPos.x, item.itemPosition.y - playerPos.y);
                if (toObstacle.magnitude <= hitRadius)
                {
                    item.hasHit = true;
                    if (hpBar != null)
                    {
                        hpBar.TakeDamage(hitDamage);
                    }
                }
            }
        }
    }
}

