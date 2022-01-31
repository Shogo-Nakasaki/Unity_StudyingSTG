/**
 *  @file   Player_Controller.cs
 *  @brief  プレイヤー関連の処理を記載
 *  @author ShogoN
 *  @date   2022.02.01
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 *  @clas   Player_Controller
 *  @brief  プレイヤー関連の処理を記載
 *          制限付き移動処理
 *          死亡時の処理
 */
public class Player_Controller : MonoBehaviour
{
    //! 移動スピードの設定
    private float move_speed = 0.1f;
    //! 移動処理で使用
    private Rigidbody2D m_Rigidody = null;

    // 仮死亡は点滅
    //! 点滅の再生時間
    private float m_Death_TimeMax = 5.0f;
    //! 点滅の再生タイマー
    private float m_Death_Timer = 0.0f;
    //! 点滅の周期時間
    private float m_Death_IntervalMax = 0.2f;
    //! 点滅の周期タイマー
    private float m_Death_IntervalTimer = 0.0f;
    //! 死亡時判定のフラグ
    private bool m_ALive = true;

    //! 描画spriteRender
    private SpriteRenderer m_SpriteRenderer;

    //! ファンネルの設定(後で消すかも)
    [SerializeField] GameObject funnel;

    /** --------------------------------------------------
     * @fn      Start
     * @brief   本クラスの処理開始時にシステムから呼び出される
     -------------------------------------------------- */
    private void Start()
    {
        // 子供からSpriteRenderを取得
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Debug.AssertFormat(m_SpriteRenderer != null, "NotEXIST:Spritedrenderer in Children");

        // Rigidbodyを取得
        m_Rigidody = GetComponent<Rigidbody2D>();
        Debug.AssertFormat(m_Rigidody != null, "NotEXIST:Rigidboddy2D");
    }

    /** --------------------------------------------------
     * @fn      Update
     * @brief   本クラスの更新時にシステムから呼び出される
     -------------------------------------------------- */
    private void Update()
    {
        Player_Move();
        funnel.transform.Rotate(new Vector3(0, 0, 5));
    }


    /** --------------------------------------------------
     * @fn      Player_Move
     * @brief   プレイヤーの動き関連を記載
     -------------------------------------------------- */
    private void Player_Move()
    {
        Vector3 add_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 new_Pos = transform.position + (add_Dir.normalized * move_speed);

        // 移動制限
        // 左下と右上に点を取る
        Vector3 screen_LB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 screen_RU = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        screen_LB.x += m_SpriteRenderer.bounds.size.x / 2.0f;
        screen_RU.x -= m_SpriteRenderer.bounds.size.x / 2.0f;
        screen_LB.y += m_SpriteRenderer.bounds.size.y / 2.0f;
        screen_RU.y -= m_SpriteRenderer.bounds.size.y / 2.0f;

        // 移動制限を設けて繁栄
        m_Rigidody.position = new Vector2(
            Mathf.Clamp(new_Pos.x, screen_LB.x, screen_RU.x),
            Mathf.Clamp(new_Pos.y, screen_LB.y, screen_RU.y));

        // 加速力は常に0
        m_Rigidody.velocity = Vector2.zero;

        // 点滅タイマーが設定されていたとき
        if (0 <= m_Death_Timer)
        {
            // 点滅間隔タイマーで切り替えタイミングを判定
            if (m_Death_IntervalMax < m_Death_IntervalTimer)
            {
                // 表示切替と点滅感覚タイマーを初期化
                Set_Visible(!m_ALive);
                m_Death_IntervalTimer = 0;
            }
            // タイマーを進める
            m_Death_IntervalTimer += Time.deltaTime;
            m_Death_Timer -= Time.deltaTime;

            if (m_Death_Timer <= 0)
            {
                Set_Visible(true);
            }
        }
    }

    /** --------------------------------------------------
     * @fn      Visible
     * @brief   Sprite表示のオンとオフを設定する。
     * @param[in] visible 表示フラグ
     -------------------------------------------------- */
    private void Set_Visible(bool visible)
    {
        m_ALive = visible;
        m_SpriteRenderer.enabled = m_ALive;
    }

    /** --------------------------------------------------
     * @fn      OntriggerEnter2D
     * @brief   コリジョン接触時のイベント処理
     * @param[in] collision 当たった側のコリジョン情報
     -------------------------------------------------- */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_Death_Timer <= 0)
        {
            m_Death_IntervalTimer = 0;
            m_Death_Timer = m_Death_TimeMax;
        }
    }
}
