/**
 *  @file   BulletState.cs
 *  @brief  弾種に関連の処理を記載
 *  @author S.Nakasaki
 *  @date   2022.02.10
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletState : MonoBehaviour
{
    //!<    発射する弾のプレハブ
    [SerializeField] public Bullet PrefabBullet;

    //!<    弾を発射するための間隔配列
    [SerializeField] public float[] TimeBulletIntervalMaxs = { 0.5f };
    //!<    現在再生中のTimeBulletIntervalMaxs配列のインデックス
    private int _m_IntervalIndex = 0;
    //!<    間隔チェック用のタイマー
    private float _m_TimeBulletInterval;

    //!<    開始時の発射向き（オイラー）
    [SerializeField] public float StartRotateSelf = 180.0f;
    //!<    回転用の加算処理（オイラー）
    [SerializeField] public float AddRotateSelf = 0.0f;
    //!<    現在の回転値
    private float _m_RotateSelf = 0.0f;

    //!<    弾自身の追加回転
    [System.NonSerialized] public float BulletAddRotate = 0.0f;
    //!<    弾自身の初速度
    [SerializeField] public float BulletStartSpeed = 1.0f;
    //!<    弾自身の終速度
    [SerializeField] public float BulletEndSpeed = 1.0f;
    //!<    弾自身の初速から終速までの時間
    [SerializeField] public float BulletTimeSpeed = 1.0f;
    //!<    弾のカラー値
    [SerializeField] public Color[] BulletColors = { new Color(1, 1, 1) };
    //!<    現在再生中のBulletColors配列のインデックス
    private int BbulletColorIndex;


    //!<    同時に発生させる弾の数
    [SerializeField] public int SameBulletCount = 1;
    //!<    同時に弾を発射させる際の角度
    [SerializeField] public float SameBulletRotate = 0.0f;


    /** -----------------------------------------------------------
    * @fn     Start
    * @brief  本クラスのコンポーネントの初期化時にシステムから呼ばれる
    -------------------------------------------------------------*/
    private void Start()
    {
        //  初回の角度を設定
        _m_RotateSelf = StartRotateSelf;
    }

    /** -----------------------------------------------------------
    * @fn     Update
    * @brief  本クラスのコンポーネントの更新時にシステムから呼ばれる
    -------------------------------------------------------------*/
    private void Update()
    {
        //  発射基準方向を更新
        _m_RotateSelf += (AddRotateSelf * Time.deltaTime);

        //  発射間隔タイマーを更新
        //  発射間隔時間を取得　指定がない場合は保険で0.5s
        _m_TimeBulletInterval += Time.deltaTime;
        float innterval = 0.5f;
        if (0 < TimeBulletIntervalMaxs.Length)
            innterval = TimeBulletIntervalMaxs[_m_IntervalIndex];

        //  発射チェック
        if (innterval < _m_TimeBulletInterval)
        {
            //  次の発射処理へ＆最後まで再生したらループ
            ++_m_IntervalIndex;
            if (_m_IntervalIndex == TimeBulletIntervalMaxs.Length) _m_IntervalIndex = 0;

            //  カラー値を取得する
            Color color = new Color(1, 1, 1);
            if (0 < BulletColors.Length)
            {
                color = BulletColors[BbulletColorIndex++];
                if (BbulletColorIndex == BulletColors.Length) BbulletColorIndex = 0;
            }

            //	弾発射
            BulletShot(
                SameBulletCount, SameBulletRotate, PrefabBullet,
                _m_RotateSelf, BulletAddRotate,
                BulletStartSpeed, BulletEndSpeed, BulletTimeSpeed,
                color);


            _m_TimeBulletInterval = 0.0f;
        }

    }

    /** -----------------------------------------------------------
    * @fn			BuleltShot
    * @brief		弾の発射処理(同時に複数対応)
	* @param[in]	nWay		同時に発射する弾数
	* @param[in]	nWayEuler	同時に複数発射した際の幅角度（オイラー角）
	* @param[in]	prefab		弾のプレハブデータ
	* @param[in]	rotateSelf	基準の角度
	* @param[in]	addRotate	弾に対して追加する角度
	* @param[in]	speedStart	弾に対して初速
	* @param[in]	speedEnd	弾に対して終速
	* @param[in]	speedTimer	弾に対しの初速から終速への時間
	* @param[in]	color		弾の色指定
    -------------------------------------------------------------*/
    private void BulletShot(int nWay, float nWayEuler, Bullet prefab, float rotateSelf, float addRotate, float speedStart, float speedEnd, float speedTimer, Color color)
    {

        //  同時発射数が１以下か２以上かで処理を変える
        if (nWay <= 1)
        {
            //  同時発射数が０以下の場合は単発処理
            CreateBullet(prefab, rotateSelf, addRotate, speedStart, speedEnd, speedTimer, color);
        }
        else
        {
            //  同時発射数が２以上の場合は発射回転幅を使って均等に発射する
            float startSomeRotate = -(nWayEuler / 2.0f);
            float addSomeRotate = nWayEuler / (nWay - 1);

            //	360度以上の場合は360度として扱う
            //	また、360度の場合は始めと最後の弾の角度が被るため数で等間隔に変更
            if (360 <= nWayEuler) { addSomeRotate = 360 / nWay; }

            //	弾の数分
            for (int i = 0; i < nWay; ++i)
            {
                //	弾の発射方向を計算する
                float rotate = rotateSelf + startSomeRotate + (addSomeRotate * i);
                //	弾を生成
                CreateBullet(prefab, rotate, addRotate, speedStart, speedEnd, speedTimer, color);
            }
        }
    }

    /** -----------------------------------------------------------
    * @fn			CreateBullet
    * @brief		弾の作成処理
	* @param[in]	prefab		弾のプレハブデータ
	* @param[in]	rotateSelf	基準の角度
	* @param[in]	addRotate	弾に対して追加する角度
	* @param[in]	speedStart	弾に対して初速
	* @param[in]	speedEnd	弾に対して終速
	* @param[in]	speedTimer	弾に対しの初速から終速への時間
	* @param[in]	color		弾の色指定
    -------------------------------------------------------------*/
    private void CreateBullet(Bullet prefab, float rotateSelf, float addRotate, float speedStart, float speedEnd, float speedTimer, Color color)
    {
        //	弾インスタンスを生成
        Bullet obj = GameObject.Instantiate<Bullet>(prefab);
        //	弾に対して必要な情報をセットアップ
        obj.Setup(rotateSelf, addRotate, speedStart, speedEnd, speedTimer, color);
        //	座標を再セット(zの位置は固定)
        Vector3 pos = transform.position; pos.z = 0.1f;
        obj.transform.position = pos;
    }
}
