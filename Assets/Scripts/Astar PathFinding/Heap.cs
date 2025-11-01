using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> 
{
    T[] items;
    int currentItemCount;
    
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }
    void SortUp(T item)
    {
        while (true)
        {
        int parentIndex = ( item.HeapIndex - 1 )/ 2;
            if (parentIndex < 0) break;
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else { break; }

            //parentIndex=(item.HeapIndex - 1 )/ 2;
           
        }
    }

    void Swap(T A,T B)
    {
        items[A.HeapIndex] = B;
        items[B.HeapIndex] = A;
        int t=A.HeapIndex;
        A.HeapIndex=B.HeapIndex;
        B.HeapIndex=t;
    }

    public T RemoveAtFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;

    }
    void SortDown(T item)
    {
        while (true)
        {
            int Left=item.HeapIndex*2+1;
            int Right=item.HeapIndex*2+2;
            int swapIdx = 0;
            if (Left < currentItemCount)
            {
                swapIdx = Left;
                if (Right < currentItemCount)
                {
                    if (items[Left].CompareTo(items[Right]) < 0)
                    {
                        swapIdx = Right;
                    }
                }

                if (item.CompareTo(items[swapIdx]) < 0)
                {
                    Swap(item, items[swapIdx]);
                }
                else return;
            }
            else return;
        }
    }
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }
    public int Count
    {
        get => currentItemCount;
    }
    public void UpdateItem(T item)
    {
        SortUp(item);
    }
}
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }

}

