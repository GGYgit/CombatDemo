using System;
using UnityEngine;

namespace Framework.Combat.Runtime{
	public static class Extensions{
		/// <summary>
		/// 将输入的移动映射到基于摄像机方向的空间
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		// 输入方向映射到世界方向
		public static Vector3 GetCameraRelativeMovement(this Vector2 input){
			Transform cameraTran = Camera.main.transform;
			Vector3 camForward = cameraTran.forward;
			Vector3 camRight = cameraTran.right;
			camForward.y = 0;
			camRight.y = 0;
			camForward.Normalize();
			camRight.Normalize();
			Vector3 moveDir = camForward * input.y + camRight * input.x;
			return moveDir;
		}

		public static Vector3 GetCameraRelativeDirection(this Vector2 input){
			Transform cameraTran = Camera.main.transform;
			var right = cameraTran.right;
			right.y = 0;
			var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
			return (input.x * right) + (input.y * forward);
		}

		public static string InsertSpaceBeforeUpperCase(this string input){
			var result = "";
			foreach (char c in input){
				if (char.IsUpper(c)){
					// if not the first letter, insert space before uppercase
					if (!string.IsNullOrEmpty(result)){
						result += " ";
					}
				}
				// start new word
				result += c;
			}
			return result;
		}

		public static string RemoveUnderline(this string input){
			return input.Replace("_", "");
		}

		/// <summary>
		/// Clear string spaces and turn to Upper
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static string ToClearUpper(this string target){
			return target.Replace(" ", string.Empty).ToUpper();
		}

		/// <summary>
		/// Check if Value is inside a range  (greater equals min && less equals than Max)
		/// </summary>
		/// <param name="value">value to compare</param>
		/// <param name="min">min range</param>
		/// <param name="max">max range</param>
		/// <returns></returns>
		public static bool IsInSideRange(this float value, float min, float max){
			return value >= min && value <= max;
		}

		/// <summary>
		/// Check if Value is inside a range (greater equals min && less equals than Max)
		/// </summary>
		/// <param name="value">value to compare</param>
		/// <param name="minMaxRange">range (x min,y max)</param>
		/// <returns></returns>
		public static bool IsInSideRange(this float value, Vector2 minMaxRange){
			return value >= minMaxRange.x && value <= minMaxRange.y;
		}

		public static bool IsVectorNaN(this Vector3 vector){
			return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
		}


		public static void SetLayerRecursively(this GameObject obj, int layer){
			obj.layer = layer;
			foreach (Transform child in obj.transform){
				child.gameObject.SetLayerRecursively(layer);
			}
		}

		public static bool ContainsLayer(this LayerMask layermask, int layer){
			return layermask == (layermask | (1 << layer));
		}

		public static void SetActiveChildren(this GameObject gameObjet, bool value){
			foreach (Transform child in gameObjet.transform){
				child.gameObject.SetActive(value);
			}
		}

		/// <summary>
		/// Check if Transfom is children
		/// </summary>
		/// <param name="me"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static bool isChild(this Transform me, Transform target){
			if (!target){
				return false;
			}
			var objName = target.gameObject.name;
			var obj = me.FindChildByNameRecursive(objName);
			if (obj == null){
				return false;
			} else{
				return obj.Equals(target);
			}
		}

		public static Transform FindChildByNameRecursive(this Transform me, string name){
			if (me.name == name){
				return me;
			}
			for (int i = 0; i < me.childCount; i++){
				var child = me.GetChild(i);
				var found = child.FindChildByNameRecursive(name);
				if (found != null){
					return found;
				}
			}
			return null;
		}


		/// <summary>
		/// Normalized the angle. between -180 and 180 degrees
		/// </summary>
		/// <param Name="eulerAngle">Euler angle.</param>
		public static Vector3 NormalizeAngle(this Vector3 eulerAngle){
			var delta = eulerAngle;
			if (delta.x > 180){
				delta.x -= 360;
			} else if (delta.x < -180){
				delta.x += 360;
			}
			if (delta.y > 180){
				delta.y -= 360;
			} else if (delta.y < -180){
				delta.y += 360;
			}
			if (delta.z > 180){
				delta.z -= 360;
			} else if (delta.z < -180){
				delta.z += 360;
			}
			return new Vector3(delta.x, delta.y, delta.z); //round values to angle;
		}

		public static Vector3 Difference(this Vector3 vector, Vector3 otherVector){
			return otherVector - vector;
		}

		public static Vector3 AngleFormOtherDirection(this Vector3 directionA, Vector3 directionB){
			return Quaternion.LookRotation(directionA).eulerAngles
				.AngleFormOtherEuler(Quaternion.LookRotation(directionB).eulerAngles);
		}

		public static Vector3 AngleFormOtherDirection(this Vector3 directionA, Vector3 directionB, Vector3 up){
			return Quaternion.LookRotation(directionA, up).eulerAngles
				.AngleFormOtherEuler(Quaternion.LookRotation(directionB, up).eulerAngles);
		}

		public static Vector3 AngleFormOtherEuler(this Vector3 eulerA, Vector3 eulerB){
			Vector3 angles = eulerA.NormalizeAngle().Difference(eulerB.NormalizeAngle()).NormalizeAngle();
			return angles;
		}

		public static string ToStringColor(this bool value){
			if (value) return "<color=green>YES</color>";
			else return "<color=red>NO</color>";
		}

		public static float ClampAngle(float angle, float min, float max){
			do{
				if (angle < -360){
					angle += 360;
				}
				if (angle > 360){
					angle -= 360;
				}
			} while (angle < -360 || angle > 360);
			return Mathf.Clamp(angle, min, max);
		}

		public static T[] Append<T>(this T[] arrayInitial, T[] arrayToAppend){
			if (arrayToAppend == null){
				throw new ArgumentNullException("The appended object cannot be null");
			}
			if ((arrayInitial is string) || (arrayToAppend is string)){
				throw new ArgumentException("The argument must be an enumerable");
			}
			T[] ret = new T[arrayInitial.Length + arrayToAppend.Length];
			arrayInitial.CopyTo(ret, 0);
			arrayToAppend.CopyTo(ret, arrayInitial.Length);
			return ret;
		}

		public static Vector3 BoxSize(this BoxCollider boxCollider){
			var length = boxCollider.transform.lossyScale.x * boxCollider.size.x;
			var width = boxCollider.transform.lossyScale.z * boxCollider.size.z;
			var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
			return new Vector3(length, height, width);
		}

		public static bool IsClosed(this BoxCollider boxCollider, Vector3 position, Vector3 margin,
			Vector3 centerOffset){
			var size = boxCollider.BoxSize();
			var marginX = margin.x;
			var marginY = margin.y;
			var marginZ = margin.z;
			var center = boxCollider.center + centerOffset;
			Vector2 rangeX = new Vector2((center.x - (size.x * 0.5f)) - marginX,
				(center.x + (size.x * 0.5f)) + marginX);
			Vector2 rangeY = new Vector2((center.y - (size.y * 0.5f)) - marginY,
				(center.y + (size.y * 0.5f)) + marginY);
			Vector2 rangeZ = new Vector2((center.z - (size.z * 0.5f)) - marginZ,
				(center.z + (size.z * 0.5f)) + marginZ);
			position = boxCollider.transform.InverseTransformPoint(position);
			bool inX = (position.x * boxCollider.transform.lossyScale.x).IsInSideRange(rangeX);
			bool inY = (position.y * boxCollider.transform.lossyScale.y).IsInSideRange(rangeY);
			bool inZ = (position.z * boxCollider.transform.lossyScale.z).IsInSideRange(rangeZ);
			return inX && inY && inZ;
		}

		public static T ToEnum<T>(this string value, bool ignoreCase = true){
			return (T) Enum.Parse(typeof(T), value, ignoreCase);
		}

		public static bool Contains<T>(this Enum value, Enum lookingForFlag) where T : struct{
			int intValue = (int) (object) value;
			int intLookingForFlag = (int) (object) lookingForFlag;
			return ((intValue & intLookingForFlag) == intLookingForFlag);
		}
	}
}
