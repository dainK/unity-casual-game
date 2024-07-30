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
                    copy[kvp.Key] = (TValue)(object)nestedDict.DeepCopy(); // ��������� ��ī��
                }
                else if (kvp.Value is ICloneable cloneable)
                {
                    copy[kvp.Key] = (TValue)cloneable.Clone(); // ICloneable�� ������ ��ü�� Clone �޼��带 ���
                }
                else
                {
                    copy[kvp.Key] = kvp.Value; // �ܼ� Ÿ���� ��� ���� ����
                }
            }
            return copy;
        }
    }
}