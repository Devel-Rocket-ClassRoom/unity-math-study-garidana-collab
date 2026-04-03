// =============================================================================
// DotProductDemo.cs
// -----------------------------------------------------------------------------
// 내적을 이용해 객체가 시야각 범위 안에 있는지 판정하는 데모
// =============================================================================

using UnityEngine;
using TMPro;

public class DotProductDemo : MonoBehaviour
{
    [Header("=== 시야 설정 ===")]
    [Tooltip("시야각(Field of View) — 전체 각도 (기본 120도)")]
    [Range(10f, 360f)]
    [SerializeField] private float fieldOfView = 120f;

    [Tooltip("시야 거리 (이 범위 안에 있어야 감지)")]
    [Range(1f, 50f)]
    [SerializeField] private float viewDistance = 10f;

    [Header("=== 대상 설정 ===")]
    [Tooltip("감지할 대상 (직접 지정하거나, 비워두면 'Enemy' 태그로 자동 탐색)")]
    [SerializeField] private Transform target; // 감지할 대상 (적 오브젝트)

    [Header("=== 시각화 색상 ===")]
    [SerializeField] private Color colorInSight = Color.green;
    [SerializeField] private Color colorOutOfSight = Color.red;
    [SerializeField] private Color colorFOV = new Color(1f, 1f, 0f, 0.5f);

    [Header("=== UI 연결 ===")]
    [Tooltip("정보 표시용 TMP_Text (Canvas 하위에 배치)")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [SerializeField] private float dotProductValue;
    [SerializeField] private float angleBetween;
    [SerializeField] private bool isInSight;

    private void Start()
    {
        if (target == null)
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
                target = enemy.transform;
            else
                Debug.LogWarning("[DotProductDemo] 'Enemy' 태그를 가진 오브젝트를 찾을 수 없습니다. " +
                    "Inspector에서 Target을 직접 지정하거나, 적 오브젝트에 'Enemy' 태그를 추가하세요.");
        }
    }

    private void Update()
    {
        if (target == null) return;

        isInSight = CheckInSight(target);

        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            targetRenderer.material.color = isInSight ? colorInSight : colorOutOfSight;
        }

        UpdateUI();
    }

    /// <summary>
    /// 대상이 시야 내에 있는지 판정하는 함수.
    /// 
    /// 작동 원리:
    /// - 1. 플레이어의 forward 방향과, 플레이어에서 대상까지의 방향 벡터를 구한다.
    /// - 2. 두 벡터의 내적(dot product)을 계산해 각도를 구한다.
    ///     - 내적이 1에 가까울수록 두 벡터가 같은 방향(0°), 0이면 수직(90°), -1이면 반대방향(180°)
    /// - 3. 대상이 시야 거리(viewDistance) 이내에 있는지 먼저 체크한다.
    /// - 4. 시야각(fieldOfView)의 절반을 코사인 값으로 계산하여 내적값과 비교한다.
    ///     - 내적값이 코사인(시야각/2)보다 크면, 대상이 시야 원뿔 안에 들어온 것으로 판정한다.
    /// - 5. dotProductValue와 angleBetween은 디버깅 및 UI 표시를 위해 저장한다.
    /// </summary>
    /// <param name="targetTransform">판정할 대상 Transform</param>
    /// <returns>시야 내에 있으면 true, 아니면 false</returns>
    private bool CheckInSight(Transform targetTransform)
    {
        // 1. 플레이어에서 대상까지의 방향 벡터 구하기
        Vector3 toTarget = targetTransform.position - transform.position;

        // 2. 대상이 시야 거리(viewDistance) 바깥에 있으면 즉시 false 반환
        if (toTarget.magnitude > viewDistance)
        {
            return false;
        }

        // 3. 방향 벡터 정규화(단위 벡터)
        Vector3 toTargetNorm = toTarget.normalized;

        // 4. 자신의 정면 방향과 대상까지의 방향의 내적(dot) 구하기
        dotProductValue = Vector3.Dot(transform.forward, toTargetNorm);

        // 5. 내적값을 각도로 변환 (코사인 역함수, 라디안->도)
        angleBetween = Mathf.Acos(dotProductValue) * Mathf.Rad2Deg;

        // 6. 시야각의 절반에 해당하는 코사인값 구하기
        // (ex: FOV=90도면 cos(45°) = 0.707...)
        float halfFovCos = Mathf.Cos(fieldOfView * 0.5f * Mathf.Deg2Rad);

        // 7. 내적값이 절반 시야각 코사인값 이상이면 시야 안에 들어옴
        return dotProductValue > halfFovCos;
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Vector3 origin = transform.position;
        Vector3 forward = Application.isPlaying ? transform.forward : transform.forward;

        float halfAngle = fieldOfView * 0.5f;
        VectorGizmoHelper.DrawFOV(origin, forward, halfAngle, viewDistance, colorFOV);

        VectorGizmoHelper.DrawArrow(origin, origin + forward * viewDistance, Color.cyan, 0.4f);

        if (target != null)
        {
            Color lineColor = isInSight ? colorInSight : colorOutOfSight;
            VectorGizmoHelper.DrawArrow(origin, target.position, lineColor, 0.3f);

            VectorGizmoHelper.DrawCircleXZ(origin, viewDistance, new Color(1f, 1f, 1f, 0.2f));

#if UNITY_EDITOR
            Vector3 midPoint = (origin + target.position) * 0.5f + Vector3.up * 0.5f;
            string info = $"Dot: {dotProductValue:F3}\n각도: {angleBetween:F1}°\n{(isInSight ? "시야 안" : "시야 밖")}";
            VectorGizmoHelper.DrawLabel(midPoint, info, lineColor);
#endif
        }
    }

    private void UpdateUI()
    {
        if (uiInfoText == null || target == null) return;

        string sightText = isInSight ? "<color=green>시야 안</color>" : "<color=red>시야 밖</color>";
        float distance = (target.position - transform.position).magnitude;

        uiInfoText.text =
            $"[DotProductDemo] 내적 시야각 판정\n" +
            $"내적(Dot) 값: {dotProductValue:F3}\n" +
            $"사이 각도: {angleBetween:F1}°  (FOV: {fieldOfView}°)\n" +
            $"판정 결과: {sightText}\n" +
            $"거리: {distance:F1} / {viewDistance}";
    }
}
