using UnityEngine;
using System.Collections;


public class BezierManager : MonoBehaviour
{
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    private float duration;
    private float elapsedTime = 0f;

    // 점들을 초기화 해주는 메서드
    public void Init (Vector3 start, Vector3 end, float time)
    {
        // 베지어 곡선의 시작점과 끝점 (정해진 좌표)지정
        p0 = start;
        p3 = end;
        duration = time;

        // 베지어 곡선의 중간 지점 p1, p2의 위치를 랜덤하게 지정
        // new Vector3 (x, y, z)에 다양한 랜덤 레인지를 부여해 이동경로가 역동적으로 생성되게 하기위함
        p1 = start + new Vector3 (Random.Range (-5f, 5f), Random.Range (2f, 8f), Random.Range (-5f, 5f));
        p2 = end + new Vector3 (Random.Range (-5f, 5f), Random.Range (2f, 8f), Random.Range(-5f, 5f));

        StartCoroutine(Move());
    }


    IEnumerator Move()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            // 베지어 곡선 공식
            transform.position = CalculateBrezierPoint(t, p0, p1, p2, p3);
            yield return null;
        }

        Destroy (gameObject);
    }

    public Vector3 CalculateBrezierPoint (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt= tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
