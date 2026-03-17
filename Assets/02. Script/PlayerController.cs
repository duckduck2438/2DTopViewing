using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float h, v;
    float moveSpeed = 5f;
    Rigidbody2D rb;
    bool isHorizontalMove = false;
    bool isGrabbing = false;
    public Animator anim;
    Vector3 vecDir;
    GameObject obj;
    Vector3 distancePlayerObj;
    float offset = 0.1f;
    public float grabDelay;
    float lastGrabTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // get axis 방향키에따라 입력값이 -1, 0, 1로 받음
        // 쯔꾸르형 게임에선 대각선 이동이 안되도록 하기 위해서 수평과 수직 중 하나의 입력만 받도록 함
        // 대화창이 열려있을때에는 움직이지 못하게 하기
        h = (GameManager.Instance.isAction) ? 0 : Input.GetAxisRaw("Horizontal");
        v = (GameManager.Instance.isAction) ? 0 : Input.GetAxisRaw("Vertical");

        // 입력 상태
        // 대화창이 열려있을때에는 움직이지 못하게 하기
        bool hDown = (GameManager.Instance.isAction) ? false : Input.GetButtonDown("Horizontal");
        bool vDown = (GameManager.Instance.isAction) ? false : Input.GetButtonDown("Vertical");
        bool hUp = (GameManager.Instance.isAction) ? false : Input.GetButtonUp("Horizontal");
        bool vUp = (GameManager.Instance.isAction) ? false : Input.GetButtonUp("Vertical");

        // 대각선 이동 방지
        if (hDown)
        {
            isHorizontalMove = true;
        }
        else if (vDown)
        {
            isHorizontalMove = false;
        }
        else if (hUp || vUp)
        {
            isHorizontalMove = h != 0;
        }

        // 최종 h, v 값 확정 (대각선 방지 및 우선순위 결정)
        if (isHorizontalMove && h != 0) v = 0;
        else if (!isHorizontalMove && v != 0) h = 0;

        // 4. 애니메이션 파라미터 전달
        // 현재 애니메이터가 들고 있는 값과 새로 입력된 값이 다를 때만 업데이트
        int curH = anim.GetInteger("hAxisRaw");
        int curV = anim.GetInteger("vAxisRaw");

        // 실제 애니메이터에 꽂아줄 목표 값 계산
        int targetH = isHorizontalMove ? (int)h : 0;
        int targetV = !isHorizontalMove ? (int)v : 0;

        if (curH != targetH || curV != targetV)
        {
            // 1. 값이 변했으므로 isChange를 true로 켜서 트랜지션 유도
            anim.SetBool("isChange", true);

            // 2. 바뀐 방향 값 전달
            anim.SetInteger("hAxisRaw", targetH);
            anim.SetInteger("vAxisRaw", targetV);
        }
        else
        {
            // 3. 값이 변하지 않은 상태(이미 걷는 중이거나 가만히 있는 중)라면 false
            anim.SetBool("isChange", false);
        }



        // raycast 방향
        if (vDown && v == 1)
        {
            vecDir = Vector3.up;
        }
        else if (vDown && v == -1)
        {
            vecDir = Vector3.down;
        }
        else if (hDown && h == 1)
        {
            vecDir = Vector3.right;
        }
        else if (hDown && h == -1)
        {
            vecDir = Vector3.left;

        }

        // game action 구현
        if (Input.GetKeyDown(KeyCode.Space) && obj != null)
        {
            GameManager.Instance.scanObject = obj;
            GameManager.Instance.Action();


        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.TogglePause();
        }


        // layer가 object인 오브젝트가 플레이어의 앞에 있는지 확인
        RaycastHit2D rayhit = Physics2D.Raycast(rb.position, vecDir, 0.7f, LayerMask.GetMask("Object"));


        if (rayhit.collider != null)
        {   // 있으면 그 오브젝트를 obj에 저장
            obj = rayhit.collider.gameObject;

            if (rayhit.collider.CompareTag("Carried"))
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastGrabTime + grabDelay)
                {
                    distancePlayerObj = obj.transform.position - transform.position;


                    Vector2 rayDirection = -rayhit.normal;

                    if (rayDirection.y == 0)
                    {
                        if (distancePlayerObj.x < 0)
                        {
                            distancePlayerObj += new Vector3(-offset, 0, 0);
                        }
                        else if (distancePlayerObj.x > 0)
                        {
                            distancePlayerObj += new Vector3(offset, 0, 0);
                        }
                    }
                    else
                    {
                        if (distancePlayerObj.y < 0)
                        {
                            distancePlayerObj += new Vector3(0, -offset, 0);
                        }
                        else if (distancePlayerObj.y > 0)
                        {
                            distancePlayerObj += new Vector3(0, offset, 0);
                        }
                    }
                    if (!isGrabbing)
                        Grab();
                    else
                    {
                        Drop();
                    }
                }

            }

        }
        else
        { // 없으면 obj는 null
            obj = null;
        }

        if (isGrabbing && obj != null)
        {
            Vector3 transPos = transform.position + distancePlayerObj;

            obj.transform.position = Vector3.Lerp(obj.transform.position, transPos, Time.deltaTime * 10f);
        }

    }

    void FixedUpdate()
    {
        // 이동 방향 설정 및 이동 구현
        Vector2 moveDir;
        if (isHorizontalMove)
        {
            moveDir = new Vector2(h, 0);
        }
        else
        {
            moveDir = new Vector2(0, v);
        }
        rb.velocity = moveDir * moveSpeed;

        //raycast
        Debug.DrawRay(rb.position, vecDir * 0.7f, Color.green);

    }

    void Grab()
    {
        isGrabbing = true;
        //anim 출력
        anim.SetBool("isGrabbing", true);

        obj.transform.SetParent(this.transform);

        Rigidbody2D grabRb = obj.GetComponent<Rigidbody2D>();
        if (grabRb != null)
        {

            grabRb.bodyType = RigidbodyType2D.Kinematic;
            grabRb.velocity = Vector2.zero;
        }

        obj.transform.position = Vector2.MoveTowards(obj.transform.position, transform.position, 0.1f);

    }


    void Drop()
    {
        isGrabbing = false;
        anim.SetBool("isGrabbing", false);
        obj.transform.SetParent(null);

        Rigidbody2D grabRb = obj.GetComponent<Rigidbody2D>();
        if (grabRb != null)
        {
            grabRb.bodyType = RigidbodyType2D.Static;
        }

        obj = null;
        lastGrabTime = Time.time;

    }

}
