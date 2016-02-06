using System;
using System.Threading;
using UnityEngine;
namespace SemaphoreThreading
{ 
    class semaforo
    {
        int totalThreads = 100;
        static int totalSemaforo = 5;
        static Semaphore sem = new Semaphore(totalSemaforo, totalSemaforo);

        public void start()
        {
            Debug.Log("called start");
            Thread[] threads = new Thread[totalThreads];
            for (int i = 0; i< totalThreads; i++)
            {
                threads[i] = new Thread(C_sharpcorner);
                threads[i].Name = "thread_" + i;
                threads[i].Start();
            }
        }

       
        static void C_sharpcorner()
        {
            Debug.Log( Thread.CurrentThread.Name + " en fila");
            sem.WaitOne();
            Debug.Log( Thread.CurrentThread.Name + " en atencion");
            Thread.Sleep(1000);
            Debug.Log( Thread.CurrentThread.Name + " saliendo");
            sem.Release();
        }
   
    }
}