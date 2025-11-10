using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveController : MonoBehaviour
{
    public SteamVR_ActionSet myAction;
    public SteamVR_Action_Vector2 JoyStick;
    public SteamVR_Action_Boolean JoyStick_Click;
    public TextMesh debug;

    private SteamVR_Input_Sources handTypeLeft;
    private SteamVR_Input_Sources handTypeRight;
    private bool rotating;

    Rigidbody rb;
    public Camera cam;
    float moveSpeed = 3f;

    // ゲーム開始時に一度だけ呼ばれる処理
    void Start()
    {
        handTypeLeft = SteamVR_Input_Sources.LeftHand;
        handTypeRight = SteamVR_Input_Sources.RightHand;
        myAction.Activate(SteamVR_Input_Sources.Any); // どっちのコントローラーでもアクションを有効化
        rb = GetComponent<Rigidbody>(); // CapsuleのRigidbodyコンポーネントを取得
    }

    // 物理演算の更新タイミングで呼ばれる処理
    void FixedUpdate()
    {
        string msg = "";
        msg += " object=" + rb.name + "\n";

        //---------------------------------------------------------------------
        // 両手のスティックの状態を取得する
        //---------------------------------------------------------------------
        Vector2 stickLeft = JoyStick.GetAxis(handTypeLeft); // 左スティックの入力値
        bool stickLeftClick = JoyStick_Click.GetState(handTypeLeft); // 左スティックのクリック状態
        Vector2 stickRight = JoyStick.GetAxis(handTypeRight); // 右スティックの入力値

        //---------------------------------------------------------------------
        // 視点の回転に関する処理（右スティック）
        //---------------------------------------------------------------------
        msg += "[rotate control info]\n";
        msg += "  x=" + stickRight.x + "\n";
        msg += "  rotating flag=" + rotating + "\n";
        msg += "  Body Quaternion=" + transform.rotation + "\n";
        msg += "  Cam Quaternion=" + cam.transform.rotation + "\n";

        // 右スティックを左に倒すと45度左に回転
        if (stickRight.x < -0.9 && rotating == false)
        {
            rotating = true; // 回転中に連続で回転しないようにフラグを立てる
            transform.Rotate(0, -45f, 0, Space.Self); // 自身を基準にY軸周りに-45度回転
        }
        // 右スティックを右に倒すと45度右に回転
        else if (stickRight.x > 0.9 && rotating == false)
        {
            rotating = true; // 回転中に連続で回転しないようにフラグを立てる
            transform.Rotate(0, 45f, 0, Space.Self); // 自身を基準にY軸周りに45度回転
        }
        // スティックが中央に戻ったら次の回転を受け付ける
        else if (stickRight.x >= -0.9 && stickRight.x <= 0.9 && rotating == true)
        {
            rotating = false;
        }

        //---------------------------------------------------------------------
        // 視点方向に移動する処理（左スティック）
        //---------------------------------------------------------------------
        msg += "[Move control info]\n";

        // カメラ（プレイヤーの視点）の前後方向を取得（Y軸方向は無視）
        Vector3 cameraForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
        msg += "  camera forward=" + cam.transform.forward + "\n";
        msg += "  calc camera forward=" + cameraForward + "\n";

        // 左スティックの入力から移動方向を計算
        // カメラの向いている方向に合わせて移動ベクトルを生成する
        Vector3 moveForward = Vector3.Scale(cameraForward * stickLeft.y + cam.transform.right * stickLeft.x, new Vector3(1, 0, 1)).normalized;
        msg += "  calc move forward=" + moveForward + "\n";

        // 左スティックがクリックされていたらダッシュ（移動速度を3倍に）
        float currentSpeed = moveSpeed;
        if (stickLeftClick)
        {
            moveSpeed *= 3.0f;
        }

        // 計算した移動方向と速度をRigidbodyに設定して移動させる
        rb.linearVelocity = moveForward * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0); // Y方向は重力に任せる
        moveSpeed = currentSpeed; // 速度を元に戻す

        //---------------------------------------------------------------------
        // Debug用のText領域が指定されていたら表示
        //---------------------------------------------------------------------
        if (debug != null)
        {
            debug.text = msg;
        }
    }
}
