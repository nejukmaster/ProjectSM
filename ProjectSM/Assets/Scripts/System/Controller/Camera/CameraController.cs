using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [System.Serializable]
    public class CameraMoveCurve
    {
        public bool trackingTarget;
        public bool stareTarget;
        public Vector3 trackingOffset;
        public BazierCurve3D.Curve curve;
    }
    [Tooltip("카메라가 쫓을 오브젝트")]
    public Transform trackingObj;
    [Tooltip("trackingObj에 카메라가 도달하는 시간")]
    [Range(1f,10f)]
    public float lerpTime;
    [Tooltip("카메라 y축 오프셋")]
    [Range(-10.0f, 10.0f)]
    public float yOffset = 0.0f;
    [Tooltip("카메라 z축 오프셋")]
    [Range(-10.0f, 10.0f)]
    public float zOffset = 0.0f;

    [Header("카메라 이펙트")]
    [Tooltip("카메라 이펙트 머티리얼")]
    [SerializeField] Material cameraEffectMat;
    [Tooltip("그레이 스케일")]
    public bool grayScale;
    [Tooltip("흔들림 효과 속도")]
    public float jiggerSpeed;

    [Header("카메라 무브")]
    [Tooltip("카메라 무브 커브 리스트")]
    public CameraMoveCurve[] movingCurves;

    Coroutine currentCo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trackingObj == null)
        {
            if(GameObject.FindWithTag("Player") != null)
                trackingObj = GameObject.FindWithTag("Player").transform;
        }
        else
        {
            Vector3 dest = new Vector3(transform.position.x, trackingObj.position.y + yOffset, trackingObj.position.z);
            if (Vector3.Distance(this.transform.position, dest) >= 0.1)
            {
                transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime * lerpTime);
            }
            if (cameraEffectMat != null)
            {
                //cameraEffectMat.SetKeyword(grayscaleKeyword,!grayScale);
                if (!grayScale)
                    cameraEffectMat.EnableKeyword("_GRAYSCALE_OFF");
                else
                    cameraEffectMat.DisableKeyword("_GRAYSCALE_OFF");
            }
        }
    }

    public void XJigger(float speed)
    {
        if (currentCo == null)
        {
            currentCo = StartCoroutine(XJiggerCo(speed));
        }
    }

    public void YJigger(float speed)
    {
        if(currentCo == null)
        {
            currentCo = StartCoroutine(YJiggerCo(speed));
        }
    }
    [ExecuteInEditMode]
    public void CameraMove(int index, float time, bool isTesting)
    {
        if(time > 0 && Mathf.Abs(index) < movingCurves.Length)
        {
            currentCo = StartCoroutine(CameraMoveCo(index, time, isTesting));
        }
    }

    IEnumerator XJiggerCo(float speed)
    {
        float n = 0f;
        while(n < Mathf.PI)
        {
            cameraEffectMat.SetFloat("_Sinx", n);
            n += Time.deltaTime * speed;
            yield return null;
        }
        currentCo = null;
        yield return null;
    }

    IEnumerator YJiggerCo(float speed)
    {
        float n = 0f;
        while (n < Mathf.PI)
        {
            cameraEffectMat.SetFloat("_Siny", n);
            n += Time.deltaTime * speed;
            yield return null;
        }
        currentCo = null;
        yield return null;
    }

    IEnumerator CameraMoveCo(int index, float time, bool isTesting)
    {
        float currentTime = 0f;
        BazierCurve3D.Curve curve = new BazierCurve3D.Curve();
        BazierCurve3D.BazierPoint[] points = new BazierCurve3D.BazierPoint[movingCurves[index].curve.points.Length+1];
        for(int i = 0; i < movingCurves[index].curve.points.Length; i++)
        {
            if (!movingCurves[index].trackingTarget)
                points[i + 1] = movingCurves[index].curve.points[i];
            else
            {
                BazierCurve3D.BazierPoint _p = movingCurves[index].curve.points[i];
                float _x = Mathf.Lerp(transform.position.x, trackingObj.position.x + movingCurves[index].trackingOffset.x, (_p.point.x - transform.position.x) / (movingCurves[index].curve.points[movingCurves[index].curve.points.Length-1].point.x - transform.position.x));
                float _y = Mathf.Lerp(transform.position.y, trackingObj.position.y + movingCurves[index].trackingOffset.y, (_p.point.y - transform.position.y) / (movingCurves[index].curve.points[movingCurves[index].curve.points.Length - 1].point.x - transform.position.y));
                float _z = Mathf.Lerp(transform.position.z, trackingObj.position.z + movingCurves[index].trackingOffset.z, (_p.point.z - transform.position.z) / (movingCurves[index].curve.points[movingCurves[index].curve.points.Length - 1].point.x - transform.position.z));
                Vector3 _p_p = new Vector3(_x, _y, _z);
                Vector3 _p_pre = _p_p + (_p.preTangent - _p.point);
                Vector3 _p_post = _p_p + (_p.postTangent - _p.point);
                points[i+1] = new BazierCurve3D.BazierPoint(_p_p,_p_post,_p_pre);
            }
        }
        points[0] = new BazierCurve3D.BazierPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                                                    new Vector3(transform.position.x, transform.position.y, transform.position.z), 
                                                    new Vector3(transform.position.x, transform.position.y, transform.position.z));
        curve.points = points;
        while(currentTime < time)
        {
            transform.position = BazierCurve3D.GetCurves(currentTime/time,curve);
            if (movingCurves[index].stareTarget)
            {
                GameObject go = new GameObject();
                go.transform.position = trackingObj.transform.position + movingCurves[index].trackingOffset;
                transform.LookAt(go.transform);
                
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        if (!isTesting)
            transform.position = movingCurves[index].curve.points[movingCurves[index].curve.points.Length - 1].point;
        else
        {
            transform.position = curve.points[0].point;
            transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
        }
    }
}
