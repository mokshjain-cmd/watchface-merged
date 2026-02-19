using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary
{
    public class LimitedStack<T>
    {

        public LimitedStack(int capacity)
        {
            this.capacity = capacity;
            stack = new Stack<T>();
        }
        int capacity { get; init; }

        Stack<T> stack;

        public void Push(T item)
        {
            if (stack.Count >= capacity)
            {
                // 当达到最大容量时，移除最早加入的元素
                RemoveFirst();
            }
           // if(stack.Select(x=> ))
            stack.Push(item);  // 后进先出，添加到末尾
        }

        void RemoveFirst() 
        {
            if (stack.Count == 0) return;

            List<T> tempStack = new List<T>();

            // 将元素从原栈转移到临时栈，但保留第一个元素
            while (stack.Count > 1)
            {
               
                tempStack.Add(stack.Pop());
            }

            // 弹出栈底元素（最早加入的元素）
            stack.Pop();
            tempStack.Reverse();
            foreach (T item in tempStack) 
            {
                stack.Push(item);
            }
            // 将元素从临时栈重新放回原栈
           
        }

        public T Pop()
        {
            
            return stack.Pop();
        }

        public int Count=>stack.Count;

        public IEnumerable<T> GetAll ()
        {
           return stack.ToArray();
        }

        public void Clear() 
        {
            stack.Clear();
        }
    }

    public class CanncelHelper 
    {
        public static LimitedStack<string> stack = new LimitedStack<string>(20);

        public static void Add(string json) 
        {
            if (!stack.GetAll().Select(x => MD5Helper.GenerateFileMD5str(x)).Contains(MD5Helper.GenerateFileMD5str(json)))
            {
                stack.Push(json);
                //Console.WriteLine("aa");
            }
        }



    }
}
