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
 *          敵の情報（キャライラストや行動）の切り替えを管理
 */

    /*
     * 行動リストを作成
     */
public class Enemy_Controller : MonoBehaviour
{
    //! 敵の行動パターンを格納するリスト
    [SerializeField] private List<EnemyState> EnemyStates;
    //! 現在の行動パターン
    private int EnemyState_Index;
    // 行動を切り替えるまでの時間
    private float EnemyState_Timer;
    //! 弾種を保持するための変数
    private List<BulletState> BulletStates = new List<BulletState>();
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class EnemyState
{
    //! 行動の時間
    [SerializeField] public float Timee;
    //! 行動中に発生する弾種
    [SerializeField] public BulletState[] BulletState;
}