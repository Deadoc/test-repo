using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPSBatteryController.Diagnostics
{
    /// <summary>
    /// Used to store and access data from any console 
    /// outputs with asyncronous access
    /// 
    /// TODO get rid of me please
    /// </summary>
    class OutputBuffer
    {
        public long LinesCount
        {
            get { return queue.Count; }
        }

        /// <summary>
        /// Read and return oldest data string
        /// </summary>
        /// <returns>null if buffer is empty now</returns>
        public string PopFront()
        {
            string res;
            queue.TryDequeue(out res);
            return res;
        }

        /// <summary>
        /// Append new data line
        /// </summary>
        /// <param name="line"></param>
        public void PushBack(string line)
        {
            queue.Enqueue(line);
        }

        public void Clear()
        {
            string item;
            while (queue.TryDequeue(out item)) ;
        }

        /// <summary>
        /// Get whole buffer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Join("\n", queue.ToList());
        }


        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    }
}
