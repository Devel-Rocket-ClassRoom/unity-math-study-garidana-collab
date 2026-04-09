// =============================================================================
// Assignment_SplineConveyor.cs
// -----------------------------------------------------------------------------
// Catmull-Rom 스플라인 위를 여러 박스가 일정 간격으로 순환 이동하는 컨베이어.
//       AnimationCurve 로 구간별 속도를 조절한다.
// =============================================================================

using UnityEngine;
using TMPro;

public class Assignment_SplineConveyor : MonoBehaviour
{
    [Header("=== 스플라인 경로 ===")]
    [SerializeField] private Transform[] waypoints;

    [Header("=== 컨베이어 박스 ===")]
    [SerializeField] private Transform[] boxes;

    [Header("=== 속도 프로파일 ===")]
    [Tooltip("x축: globalT(0~1), y축: 속도 배율. 기본 Linear는 일정 속도")]
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Range(1f, 20f)] [SerializeField] private float cycleDuration = 6f;

    [Header("=== 시각화 ===")]
    [Range(10, 100)] [SerializeField] private int splineResolution = 50;

    [Header("=== UI 연결 ===")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [SerializeField] private float globalT;
    [SerializeField] private float currentSpeedMultiplier;
    
    private void Update()
    {
        // TODO
        if (waypoints == null || waypoints.Length < 2 || boxes == null) return;
        // 속도 배율 가져오기 (AnimationCurve에서 현재 globalT 위치의 값 샘플링)
        currentSpeedMultiplier = speedCurve.Evaluate(globalT);
        // cycleDuration이 작을수록 빨리 회전, 속도 배율을 곱해 가변 속도 구현
        globalT += (Time.deltaTime / cycleDuration) * currentSpeedMultiplier;
        // globalT가 1을 넘으면 0으로 되돌려 무한 반복
        globalT %= 1f;
        // 각 박스들의 위치와 방향 설정
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] == null)
            {
                continue;
            }
            // 박스들이 일정 간격으로 배치되도록 오프셋 계산 ( i / 전체 갯수)
            float boxOffset = (float)i / boxes.Length;
            // 각 박스의 개별 t값 계산 (globalT에 오프셋을 더하고 1이 넘으면 다시 0부터 시작)   
            float boxT = (globalT + boxOffset) % 1f;

            
            // 스플라인 곡선 위의 위치 계산
            Vector3 targetPos = EvaluateSpline(waypoints, boxT);
            boxes[i].position = targetPos;

            //아주 미세한 차이 (0.01f)의 다음 위치를 구해 바라보게 함 
            float nextT = (boxT + 0.01f) % 1f;
            Vector3 lookAtPos = EvaluateSpline(waypoints, nextT);
            boxes[i].LookAt(lookAtPos);
        }
        UpdateUI();
    }

    private Vector3 EvaluateSpline(Transform[] pts, float t)
    {
        int segmentCount = pts.Length - 1;
        float scaledT = t * segmentCount;
        int segment = Mathf.Clamp((int)scaledT, 0, segmentCount - 1);
        float localT = scaledT - segment;

        Vector3 p0 = pts[Mathf.Max(0, segment - 1)].position;
        Vector3 p1 = pts[segment].position;
        Vector3 p2 = pts[Mathf.Min(pts.Length - 1, segment + 1)].position;
        Vector3 p3 = pts[Mathf.Min(pts.Length - 1, segment + 2)].position;

        return CatmullRom(p0, p1, p2, p3, localT);
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t, t3 = t2 * t;
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    private void UpdateUI()
    {
        if (uiInfoText == null) return;

        int boxCount = boxes != null ? boxes.Length : 0;
        int wpCount = waypoints != null ? waypoints.Length : 0;

        uiInfoText.text = $"[Assignment_SplineConveyor] 스플라인 컨베이어\n"
                        + $"웨이포인트: {wpCount}개 / 박스: {boxCount}개\n"
                        + $"globalT: {globalT:F2}\n"
                        + $"속도 배율: {currentSpeedMultiplier:F2}×\n"
                        + $"주기: {cycleDuration:F1}초\n"
                        + $"\n<color=yellow>AnimationCurve로 구간별 속도 조절</color>";
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enabled) return;
        if (waypoints == null || waypoints.Length < 2) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
        }

        Gizmos.color = Color.cyan;
        Vector3 prev = EvaluateSpline(waypoints, 0f);
        for (int i = 1; i <= splineResolution; i++)
        {
            float t = i / (float)splineResolution;
            Vector3 curr = EvaluateSpline(waypoints, t);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }
    }
#endif
}
