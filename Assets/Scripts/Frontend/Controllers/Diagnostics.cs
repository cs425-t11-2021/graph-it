using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diagnostics : SingletonBehavior<Diagnostics>
{
    [SerializeField] private bool printMemoryLogs = false;

    private long previousSecondMemory = 0;
    private long tenSecondsAgoMemory = 0;

    private void Awake()
    {
        if (this.printMemoryLogs)
        {
            StartCoroutine(LogMemoryUse());
            Logger.Log("Incremental GC: " + UnityEngine.Scripting.GarbageCollector.isIncremental, this, LogType.WARNING);
        }
    }

    private IEnumerator LogMemoryUse()
    {
        int iteration = 0;
        while (true)
        {
            long memory = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / (1024*1024);
            if (this.previousSecondMemory != 0)
            {
                Logger.Log(String.Format("Process Memory: {0} mb, Rate: {1} mb/s", memory, memory - this.previousSecondMemory), this, LogType.INFO);
            }

            if (iteration % 10 == 0)
            {
                if (this.tenSecondsAgoMemory != 0)
                {
                    Logger.Log(String.Format("Rate Over Last Ten Seconds: {0} mb/s", (memory - this.tenSecondsAgoMemory) / 10f), this, LogType.INFO);
                }

                this.tenSecondsAgoMemory = memory;
            }

            this.previousSecondMemory = memory;
            iteration++;
            yield return new WaitForSeconds(1);
        }

    }
}
