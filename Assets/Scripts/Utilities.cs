using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static T[] PickRandomUniqueElements<T>(T[] sourceArray, int count)
    {
        // Check if we're asking for more elements than the array contains
        if (count > sourceArray.Length)
        {
            Debug.LogWarning("Requested more elements than available in the array.");
            count = sourceArray.Length;
        }

        // Create a copy of the source array
        T[] shuffledArray = new T[sourceArray.Length];
        sourceArray.CopyTo(shuffledArray, 0);

        // Shuffle the array
        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }

        // Take the first 'count' elements
        T[] result = new T[count];
        Array.Copy(shuffledArray, result, count);

        return result;
    }
}
