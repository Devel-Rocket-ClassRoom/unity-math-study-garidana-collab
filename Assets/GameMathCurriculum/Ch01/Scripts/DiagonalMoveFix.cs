// =============================================================================
// DiagonalMoveFix.cs
// -----------------------------------------------------------------------------
// 정규화를 이용해 대각선 이동의 속도 보정을 구현하는 데모
// =============================================================================

using UnityEngine;
using TMPro;

public class DiagonalMoveFix : MonoBehaviour
{
    [Header("=== 이동 설정 ===")]
    [Tooltip("이동 속도 (units/sec)")]
    [Range(1f, 20f)]
    [SerializeField] private float moveSpeed = 5f;

    [Header("=== 보정 토글 ===")]
    [Tooltip("true: 정규화 적용 (올바른 대각선 속도)\nfalse: 정규화 미적용 (대각선이 √2배 빠름)")]
    [SerializeField] private bool useNormalized = false;

    [Header("=== UI 연결 ===")]
    [Tooltip("정보 표시용 TMP_Text (Canvas 하위에 배치)")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [Tooltip("현재 프레임의 입력 방향 벡터")]
    [SerializeField] private Vector3 currentInputDirection;

    [Tooltip("현재 프레임의 입력 벡터 크기")]
    [SerializeField] private float currentInputMagnitude;

    [Tooltip("현재 프레임의 실제 이동 속도")]
    [SerializeField] private float currentSpeed;

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");   
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(h, 0f, v);  // 입력 방향 벡터


        if (useNormalized)
        {
            moveDirection.Normalize();
        }

        // moveDirection(플레이어가 누른 방향키 조합, 예: (1,0,1)이면 오른쪽+앞쪽)의 크기는
        // 대각선일 때 1.41로 커져서 실제 속도가 1.41배 빨라진다.
        // (W+A, W+D, S+A, S+D 등 두 방향 키를 동시에 누르면 대각선)
        // Time.deltaTime은 1프레임당 걸린 시간(초), moveSpeed는 초당 이동거리.
        transform.position += moveDirection * moveSpeed * Time.deltaTime; // 이동 처리

        currentInputDirection = moveDirection;
        currentInputMagnitude = moveDirection.magnitude;
        currentSpeed = currentInputMagnitude * moveSpeed;
        // TODO

        UpdateUI();
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        // 실행중인 경우가 아니면 그리지 않음
        if (!Application.isPlaying) return;

        Vector3 origin = transform.position;

        if (currentInputDirection != Vector3.zero)
        {
            Color arrowColor = useNormalized ? Color.green : Color.red;
            VectorGizmoHelper.DrawArrow(origin, origin + currentInputDirection * 2f, arrowColor, 0.3f);
        }

        VectorGizmoHelper.DrawCircleXZ(origin, 1f, Color.white);

#if UNITY_EDITOR // 에디터 모드인 경우만 라벨 그리기
        string stateText = useNormalized ? "보정 후 (normalized)" : "보정 전 (원본)";
        string speedText = $"입력 크기: {currentInputMagnitude:F3}\n실제 속도: {currentSpeed:F2}";
        VectorGizmoHelper.DrawLabel(origin + Vector3.up * 2f, $"{stateText}\n{speedText}",
            useNormalized ? Color.green : Color.red);
#endif
    }

    private void UpdateUI()
    {
        if (uiInfoText == null) return;

        string mode = useNormalized
            ? "<color=#00FF00>보정 후 (normalized)</color>"
            : "<color=#FF0000>보정 전 (원본)</color>";

        uiInfoText.text =
            $"[DiagonalMoveFix]\n" +
            $"모드: {mode}\n" +
            $"입력 방향: {currentInputDirection}\n" +
            $"입력 크기: {currentInputMagnitude:F3} (이상적: 1.000)\n" +
            $"실제 속도: {currentSpeed:F2}\n" +
            $"Inspector에서 'Use Normalized' 토글로 비교하세요!";
    }
}
