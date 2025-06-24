using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Combat.Runtime{
	// public class FightingCircleManager
	// 	: MonoBehaviour{
	// 	private Dictionary<int, FightingCircle> circles;
	//
	// 	public FightingCircle[] debug;
	// 	public bool isDebug = false;
	//
	// 	private void Awake(){
	// 		circles = new Dictionary<int, FightingCircle>();
	// 	}
	//
	// 	public bool RegisterFightingCircle(FightingCircle circle){
	// 		int instance = circle.gameObject.GetInstanceID();
	// 		if (circles.ContainsKey(instance)){
	// 			return false;
	// 		}
	// 		circles.Add(instance, circle);
	// 		if (isDebug) debug = circles.Values.ToArray();
	// 		return true;
	// 	}
	//
	// 	public bool MoveBetweenCircle(KungFuCircleSubject attacker, KungFuCircleSubject defender, ECircles to){
	// 		int instanceDefender = defender.GetInstanceID();
	// 		var circle = circles[instanceDefender];
	// 		return circle.Move(attacker, to);
	// 	}
	//
	// 	public Vector3 GetSlotPositionFromMeleeCircle
	// 		(KungFuCircleSubject attacker, KungFuCircleSubject defender){
	// 		int instanceDefender = defender.GetInstanceID();
	// 		if (!circles.ContainsKey(instanceDefender))
	// 			return Vector3.zero;
	// 		var circle = circles[instanceDefender];
	// 		return circle.GetSlotPositionFromMeleeCircle(attacker);
	// 	}
	//
	// 	public Pair<Vector3, Vector3> GetSlotPositionFromApproachCircle
	// 		(KungFuCircleSubject attacker, KungFuCircleSubject defender){
	// 		int instanceDefender = defender.GetInstanceID();
	// 		if (!circles.ContainsKey(instanceDefender))
	// 			return null;
	// 		var circle = circles[instanceDefender];
	// 		return circle.GetSlotPositionFromApproachCircle(attacker);
	// 	}
	//
	//
	// 	public bool RegisterActionAttack(KungFuCircleSubject attacker, KungFuCircleSubject defender, float weight){
	// 		int instanceDefender = defender.GetInstanceID();
	// 		return circles[instanceDefender].RegisterAction(attacker, weight);
	// 	}
	//
	// 	public bool UnregisterActionAttack(KungFuCircleSubject attacker, KungFuCircleSubject defender, float weight){
	// 		int instanceDefender = defender.GetInstanceID();
	// 		return circles[instanceDefender].UnregisterAction(attacker, weight);
	// 	}
	//
	// 	// public bool RegisterToCircle(KungFuCircleSubject attacker, KungFuCircleSubject defender, ECircles type){
	// 	// 	int instanceDefender = defender.GetInstanceID();
	// 	// 	return circles[instanceDefender].Register(attacker, type);
	// 	// }
	//
	//
	// 	//public Vector3 GetSlotPosition (Subject attacker, Subject defender){
	// 	//    int instanceDefender = defender.gameObject.GetInstanceID ();
	//
	// 	//    return circles[instanceDefender].GetSlotPosition (attacker);
	// 	//}
	// }
}
