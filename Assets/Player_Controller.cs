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
    [System.NonSerialized]public float move_speed = 0.1f;
    //! 移動処理で使用
    [System.NonSerialized] private Rigidbody2D Player_Rigidody = null;

    // 敵接触時、点滅
    //! 点滅の再生時間
    [System.NonSerialized] public float m_contact_TimeMax = 5.0f;
    //! 点滅の再生タイマー
    [System.NonSerialized] public float m_contact_Timer = 0.0f;
    //! 点滅の周期時間
    [System.NonSerialized] public float m_contact_IntervalMax = 0.2f;
    //! 点滅の周期タイマー
    [System.NonSerialized] public float m_contact_IntervalTimer = 0.0f;
    //! 接触判定のフラグ
    [System.NonSerialized] public bool m_contact = true;

    //! 描画spriteRender
    [System.NonSerialized] private SpriteRenderer m_SpriteRenderer;

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
        Player_Rigidody = GetComponent<Rigidbody2D>();
        Debug.AssertFormat(Player_Rigidody != null, "NotEXIST:Rigidboddy2D");
    }

    /** --------------------------------------------------
     * @fn      Update
     * @brief   本クラスの更新時にシステムから呼び出される
     -------------------------------------------------- */
    private void Update()
    {
        Player_Move();
        // funnel.transform.Rotate(new Vector3(0, 0, 0.5f));
    }


    /** --------------------------------------------------
     * @fn      Player_Move
     * @brief   プレイヤーの動き関連を記載
     -------------------------------------------------- */
    private void Player_Move()
    {
        Vector3 addDirect = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 new_Position = transform.position + (addDirect.normalized * move_speed);

        // 移動制限
        // 左下と右上に点を取る
        Vector3 screen_LB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 screen_RU = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        screen_LB.x += m_SpriteRenderer.bounds.size.x / 4.0f;
        screen_RU.x -= m_SpriteRenderer.bounds.size.x / 4.0f;
        screen_LB.y += m_SpriteRenderer.bounds.size.y / 3.0f;
        screen_RU.y -= m_SpriteRenderer.bounds.size.y / 3.0f;

        // 移動制限を設けて反映
        Player_Rigidody.position = new Vector2(
            Mathf.Clamp(new_Position.x, screen_LB.x, screen_RU.x),   // X座標の制限
            Mathf.Clamp(new_Position.y, screen_LB.y, screen_RU.y)    // Y座標の制限
            );

        // 加速力は常に0
        Player_Rigidody.velocity = Vector2.zero;

        // 点滅タイマーが設定されていたとき
        if (0 <= m_contact_Timer)
        {
            // 点滅間隔タイマーで切り替えタイミングを判定
            if (m_contact_IntervalMax < m_contact_IntervalTimer)
            {
                // 表示切替と点滅感覚タイマーを初期化
                Set_Visible(!m_contact);
                m_contact_IntervalTimer = 0;
            }
            // タイマーを進める
            m_contact_IntervalTimer += Time.deltaTime;
            m_contact_Timer -= Time.deltaTime;

            if (m_contact_Timer <= 0)
            {
                Set_Visible(true);
                Debug.Log("点滅終了");
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
        m_contact = visible;
        m_SpriteRenderer.enabled = m_contact;
    }

    /** --------------------------------------------------
     * @fn      OntriggerEnter2D
     * @brief   コリジョン接触時のイベント処理
     * @param[in] collision 当たった側のコリジョン情報
     -------------------------------------------------- */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_contact_Timer <= 0)
        {
            m_contact_IntervalTimer = 0;
            m_contact_Timer = m_contact_TimeMax;
        }
    }
}
