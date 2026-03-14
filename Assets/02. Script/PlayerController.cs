using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float h, v;
    float moveSpeed = 5f;
    Rigidbody2D rb;
    bool isHorizontalMove = false;
    public Animator anim;
    Vector3 vecDir;
    GameObject obj;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // get axis 방향키에따라 입력값이 -1, 0, 1로 받음
        // 쯔꾸르형 게임에선 대각선 이동이 안되도록 하기 위해서 수평과 수직 중 하나의 입력만 받도록 함
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        // 입력 상태
        bool hDown = Input.GetButtonDown("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");
        bool hMove = Input.GetButton("Horizontal");
        bool vMove = Input.GetButton("Vertical");

        // 대각선 이동 방지
        if (hDown || vUp)
        {
            isHorizontalMove = true;
        }
        else if (vDown || hUp)
        {
            isHorizontalMove = false;
        }

        // 반대키 눌렀을때 조작감 완화
        else if (hMove)
        {
            if (vMove)
            {
                isHorizontalMove = false;
                return;
            }
            isHorizontalMove = true;
        }
        else if (vMove)
        {
            if (hMove)
            {
                isHorizontalMove = true;
                return;
            }
            isHorizontalMove = false;
        }

        // animation
        if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
        {
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
        if(Input.GetKeyDown(KeyCode.Space) && obj != null)
        {
            Debug.Log(obj.name);
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

        // layer가 object인 오브젝트가 플레이어의 앞에 있는지 확인
        RaycastHit2D rayhit = Physics2D.Raycast(rb.position, vecDir, 0.7f, LayerMask.GetMask("Object"));


        if(rayhit.collider != null)
        {   // 있으면 그 오브젝트를 obj에 저장
            obj = rayhit.collider.gameObject;
        } 
        else
        { // 없으면 obj는 null
            obj = null;
        }
    }
}
