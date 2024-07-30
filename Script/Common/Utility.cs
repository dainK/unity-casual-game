using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameHeaven
{

    public static class Utility
    {
        public static void Shuffle<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static List<T> DeepCopy<T>(this List<T> original) where T : ICloneable
        {
            var copy = new List<T>(original.Count);
            foreach (var item in original)
            {
                copy.Add((T)item.Clone());
            }
            return copy;
        }

        public static Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            var copy = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
            foreach (var kvp in original)
            {
                if (kvp.Value is Dictionary<TKey, TValue> nestedDict)
                {
                    copy[kvp.Key] = (TValue)(object)nestedDict.DeepCopy(); // 재귀적으로 딥카피
                }
                else if (kvp.Value is ICloneable cloneable)
                {
                    copy[kvp.Key] = (TValue)cloneable.Clone(); // ICloneable을 구현한 객체는 Clone 메서드를 사용
                }
                else
                {
                    copy[kvp.Key] = kvp.Value; // 단순 타입의 경우 얕은 복사
                }
            }
            return copy;
        }
    }
}