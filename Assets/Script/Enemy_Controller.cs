/**
 *  @file   Enemy_Controller.cs
 *  @brief  敵キャラ関連の処理を記載
 *  @author S.Nakasaki
 *  @date   2022.02.08
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 *  @clas   Enemy_Controller
 *  @brief  敵キャラ関連の処理を記載
 *          敵の情報（行動）の切り替えを管理
 */
public class Enemy_Controller : MonoBehaviour
{
    //!<    敵の行動パターンデータのリスト
    [SerializeField]
    private List<EnemyState> EnemyStates;


    //!<    現在再生中の行動パターン
    private int _m_EnemyStateIndex;
    //!<    切り替えるまでのタイマー
    private float _m_EnemyStateTimer;
    //!<    現在再生中の発射パターンを保持
    private List<BulletState> _m_BulletStates = new List<BulletState>();



    /** -----------------------------------------------------------
    * @fn     Start
    * @brief  本クラスのコンポーネントの初期化時にシステムから呼ばれる
    -------------------------------------------------------------*/
    private void Start()
    {
        Debug.AssertFormat(0 < EnemyStates.Count, "EnemyStates Array is 0 Size");
        //  0版の情報から
        _m_EnemyStateIndex = 0;
        ChangeEnemyState(_m_EnemyStateIndex);
    }

    /** -----------------------------------------------------------
    * @fn     Update
    * @brief  本クラスのコンポーネントの更新時にシステムから呼ばれる
    -------------------------------------------------------------*/
    private void Update()
    {
        //  行動パターンがない場合は何もしない
        if (EnemyStates.Count == 0) return;

        //  行動タイマーチェック(指定が０の場合は何もしない)
        if (_m_EnemyStateTimer <= 0 && 0.0f < EnemyStates[_m_EnemyStateIndex].Time)
        {
            //  指定時間経過した場合は次のパターンに切り替える
            ++_m_EnemyStateIndex;
            if (EnemyStates.Count <= _m_EnemyStateIndex)
            {   //  猥語まで再生した場合は初めに戻す
                _m_EnemyStateIndex = 0;
            }
            //  あらたなアクションパターンを更新
            ChangeEnemyState(_m_EnemyStateIndex);

        }
        //  タイマーを進める
        _m_EnemyStateTimer -= Time.deltaTime;
    }

    /** -----------------------------------------------------------
    * @fn        ChangeEnemyState
    * @brief     敵の行動パターンを切り替える
    * @param[in] enemyStateIndex 次に切り替えるインデックス
    -------------------------------------------------------------*/
    private void ChangeEnemyState(int enemyStateIndex)
    {
        //  TODO. y-hashimoto enemyStatesの要素数が０個の場合はエラーを出す

        //  現在再生中の行動パターンを削除＆リストもクリア
        foreach (var item in _m_BulletStates)
            Destroy(item.gameObject);
        _m_BulletStates.Clear();

        //  新たな行動パターンを作成
        foreach (var item in EnemyStates[enemyStateIndex].BulletState)
        {
            var obj = Instantiate(item, transform);         //  オブジェクト作成
            obj.transform.parent = transform;               //  作成したオブジェクトを本オブジェクトの子に設定
            obj.transform.position = transform.position;    //  座標を同じ位置に設定
            _m_BulletStates.Add(obj);                       //  管理リストに登録
        }
        //  タイマーを再設定
        _m_EnemyStateTimer = EnemyStates[enemyStateIndex].Time;
    }
}


/** -----------------------------------------------------------
 * @class  EnemyState
 * @brief  敵の行動パターンデータ定義
 *         １単位としての行動
-------------------------------------------------------------*/
[System.Serializable]
public class EnemyState
{
    //!<    行動の時間
    [SerializeField]
    public float Time;
    //!<    行動中に発生させる弾の発射種類（配列内すべて同時に発生させる）
    [SerializeField]
    public BulletState[] BulletState;
}
