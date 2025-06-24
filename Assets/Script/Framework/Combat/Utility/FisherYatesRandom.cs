using System.Collections.Generic;
using UnityEngine;

namespace Framework.Combat.Runtime{
	// Fisher-Yates 洗牌算法
	public class FisherYatesRandom{
		private int[] randomIndices = null;
		private int randomIndex = 0;
		private int prevValue = -1;

		public int Next(int len){
			if (len <= 1)
				return 0;
			if (randomIndices == null || randomIndices.Length != len){
				randomIndices = new int[len];
				for (int i = 0; i < randomIndices.Length; i++)
					randomIndices[i] = i;
			}
			if (randomIndex == 0){
				int count = 0;
				do{
					for (int i = 0; i < len - 1; i++){
						int j = Random.Range(i, len);
						if (j != i){
							(randomIndices[i], randomIndices[j]) = (randomIndices[j], randomIndices[i]);
						}
					}
				} while
					(prevValue == randomIndices[0] &&
					 ++count < 10);
			}
			int value = randomIndices[randomIndex];
			if (++randomIndex >= randomIndices.Length)
				randomIndex = 0;
			prevValue = value;
			return value;
		}

		public int Range(int min, int max){
			var len = (max - min) + 1;
			if (len <= 1)
				return max;
			if (randomIndices == null || randomIndices.Length != len){
				randomIndices = new int[len];
				for (int i = 0; i < randomIndices.Length; i++)
					randomIndices[i] = min + i;
			}
			if (randomIndex == 0){
				int count = 0;
				do{
					for (int i = 0; i < len - 1; i++){
						int j = Random.Range(i, len);
						if (j != i){
							(randomIndices[i], randomIndices[j]) = (randomIndices[j], randomIndices[i]);
						}
					}
				} while
					(prevValue == randomIndices[0] &&
					 ++count < 10);
			}
			int value = randomIndices[randomIndex];
			if (++randomIndex >= randomIndices.Length)
				randomIndex = 0;
			prevValue = value;
			return value;
		}

		public static void Shuffle<T>(List<T> list){
			System.Random random = new System.Random();
			// 洗牌
			for (int i = list.Count - 1; i >= 0; i--){
				int transIndex = random.Next(0, i); // 乱数 0 ~ (i - 1)
				// 互換位置
				T temp = list[transIndex];
				list[transIndex] = list[i];
				list[i] = temp;
			}
		}
	}
}
