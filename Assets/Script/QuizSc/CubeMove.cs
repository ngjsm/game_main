// CubeMove.cs
using UnityEngine;
using Leap.PhysicalHands;

[RequireComponent(typeof(Rigidbody))]
public class CubeMove : MonoBehaviour
{
    [Header("연속 이동 설정")]
    public float moveSpeed = 2f;  // 유닛/초

    [Header("버튼 복귀 설정")]
    public Vector3 initialPositionOverride = new Vector3(0, 1, 1);
    public float smoothTime = 0.2f;

    private Rigidbody rb;
    private Vector3 initialPosition;
    private Vector3 currentVelocity;
    private bool isReturning;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;

        // 초기 위치 기록
        initialPosition = initialPositionOverride;

        currentVelocity = Vector3.zero;
        isReturning = false;

        // PhysicalHandsButton 눌림 시에도 복귀
        var phys = GetComponent<PhysicalHandsButton>();
        if (phys != null)
            phys.OnButtonPressed.AddListener(ReturnToInitialSmooth);
    }

    void FixedUpdate()
    {
        if (isReturning)
        {
            // 부드럽게 initialPosition으로 복귀
            Vector3 next = Vector3.SmoothDamp(
                rb.position,
                initialPosition,
                ref currentVelocity,
                smoothTime,
                Mathf.Infinity,
                Time.fixedDeltaTime
            );
            rb.MovePosition(next);
            // 복귀가 충분히 근접하면 연산 종료
            if (Vector3.Distance(rb.position, initialPosition) < 0.01f)
                isReturning = false;
        }
        else
        {
            // 화살표 키 연속 이동
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow)) dir += Vector3.up;
            if (Input.GetKey(KeyCode.DownArrow)) dir += Vector3.down;
            if (Input.GetKey(KeyCode.LeftArrow)) dir += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow)) dir += Vector3.right;

            if (dir != Vector3.zero)
            {
                dir.Normalize();
                rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    /// <summary>
    /// 외부에서 호출하거나 버튼 눌림 시 호출해
    /// 초기 위치로 SmoothDamp 복귀를 시작합니다.
    /// </summary>
    public void ReturnToInitialSmooth()
    {
        isReturning = true;
        currentVelocity = Vector3.zero;
    }
}
