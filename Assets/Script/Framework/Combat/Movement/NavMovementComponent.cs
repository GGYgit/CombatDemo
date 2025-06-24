using UnityEngine;
using UnityEngine.AI;

namespace Framework.Combat.Runtime{
	public class NavMovementComponent : MovementComponent{
		public float moveSpeed = 2.0f;
		public float sprintSpeed = 5.335f;
		public float rotateSpeed = 180f;

		private NavMeshAgent agent;

		void Awake(){
			agent = GetComponent<NavMeshAgent>();
		}

		public void DisableAgent(){
			agent.enabled = false;
		}

		public void EnableAgent(){
			agent.enabled = true;
		}

		public override void MoveInput(Vector2 input){
			agent.Move(new Vector3(input.x, 0, input.y));
		}

		public override void Move(Vector3 offset){
			if (agent.enabled){
				agent.Move(offset);
			} else{
				transform.position += offset;
			}
		}

		public override void MoveTo(Vector3 destination){
			agent.isStopped = false;
			agent.SetDestination(destination);
		}

		public void SetMovementState(MovementStateType stateType){
			float speed = moveSpeed;
			switch (stateType){
				case MovementStateType.Jog:
					speed = moveSpeed;
					break;
				case MovementStateType.Sprint:
					speed = sprintSpeed;
					break;
			}
			agent.speed = speed;
			agent.angularSpeed = rotateSpeed;
		}

		public void Stop(){
			agent.isStopped = true;
		}

		public bool HasReachedDestination(float arriveDistance = 0.5f){
			if (!agent.enabled) return false;
			if (!agent.pathPending && agent.remainingDistance <= arriveDistance){
				return true;
			}
			return false;
		}
	}
}
