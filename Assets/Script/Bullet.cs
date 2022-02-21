/**
 *  @file   BulletState.cs
 *  @brief  弾単体に処理を記載
 *  @author S.Nakasaki
 *  @date   2022.02.10
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  @class  Bullet
 *  @brief  弾単体の処理を記載
 *          弾が画面外に出ると消す
 */
public class Bullet : MonoBehaviour
{
    //  開始時の弾の発射角度
    private float StartRotate;
    //  追加の回転値
    private float AddRotate;
    //  追加の回転値
    private float Rotate;
    //  初速度
    private float StartSpeed = 1.0f;
    //  終速度
    private float EndSpeed;
    //  初速から終速までの時間
    private float TimeSpeedMax;
    //  弾のスピードタイマー
    private float TimeSpeedTimer;


    /** -----------------------------------------------------------
    * @fn        Setup
    * @brief     各弾パラメータのセットアップ
    * @param[in] rotate 開始向き
    * @param[in] speed  移動スピード
    -------------------------------------------------------------*/
    public void Setup( float rotate , float speed )
    {
        Setup(rotate , 0 , speed , speed , 0.0f , new Color(1 , 1 , 1 ));
    }

    /** -----------------------------------------------------------
    * @fn        Setup
    * @brief     各弾パラメータのセットアップ
    * @param[in] startRotate 開始向き
    * @param[in] addRotate   追加回転
    * @param[in] startSpeed  初速
    * @param[in] endSpeed    終速
    * @param[in] timeSpeed   初速から終速までの時間
    * @param[in] color       弾の色
    -------------------------------------------------------------*/
    public void Setup(float startRotate , float addRotate , float startSpeed , float endSpeed , float timeSpeed , Color color)
    {
        StartRotate = startRotate;
        AddRotate = Mathf.Min(addRotate , 20.0f);
        StartSpeed = Mathf.Max(timeSpeed, startSpeed, 0.1f);
        EndSpeed = Mathf.Max(timeSpeed, endSpeed, 0.1f);
        TimeSpeedMax = Mathf.Max( timeSpeed , 0.1f);

        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        if(render)
        {
            color.a = 1.0f;
            render.color = color;
        }

    }


	/** -----------------------------------------------------------
    * @fn     Update
    * @brief  本クラスのコンポーネントの更新時にシステムから呼ばれる
    -------------------------------------------------------------*/
	private void Update()
    {
        //  弾の向きを更新＆設定
        Rotate += (AddRotate * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, StartRotate + Rotate));


        //  移動スピードを設定
        TimeSpeedTimer = Mathf.Min(TimeSpeedTimer + Time.deltaTime, TimeSpeedMax);
        float speed = Mathf.Lerp( StartSpeed , EndSpeed , TimeSpeedTimer / TimeSpeedMax);
        transform.position += transform.up * speed * Time.deltaTime;


        // 画面外に出たかどうか判定
        // 画面の左下の座標を取得 (左上じゃないので注意)
        // 画面の右上の座標を取得 (右下じゃないので注意)
        Vector3 screen_LB = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 screen_RU = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        if (transform.position.x < screen_LB.x || screen_RU.x < transform.position.x ||
            transform.position.y < screen_LB.y || screen_RU.y < transform.position.y)
        {
            Destroy(this.gameObject);
        }

    }
}
