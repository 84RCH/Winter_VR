using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem; // SteamVRの機能を使うために必要

public class ColliderChanger : MonoBehaviour
{
    // 物を掴んだ時に呼ばれる
    private void OnAttachedToHand(Hand hand)
    {
        Debug.Log("OnAttachedToHand:" + gameObject.name + "/handType:" + hand.handType);
        // 掴んだ物のCollider（衝突判定）を「isTrigger」にする
        // isTriggerがtrueだと、他のColliderと物理的な衝突をしなくなる
        GetComponent<Collider>().isTrigger = true;
    }

    // 物を離した時に呼ばれる
    private void OnDetachedFromHand(Hand hand)
    {
        Debug.Log("OnDetachedFromHand:" + gameObject.name + "/handType:" + hand.handType);
        // 物を離したら、ColliderのisTriggerを元に戻す
        GetComponent<Collider>().isTrigger = false;
    }

    // （参考：これらのメソッドは今回は必須ではありませんが、手の状態をデバッグする際に便利です）
    private void OnHandHoverBegin(Hand hand)
    {
        Debug.Log("OnHandHoverBegin:" + gameObject.name + "/handType:" + hand.handType);
    }

    private void OnHandHoverEnd(Hand hand)
    {
        Debug.Log("OnHandHoverEnd:" + gameObject.name + "/handType:" + hand.handType);
    }
}
