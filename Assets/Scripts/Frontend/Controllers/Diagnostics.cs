using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;

public class Diagnostics : SingletonBehavior<Diagnostics>
{
    [SerializeField] private bool printMemoryLogs = false;

    private long previousSecondMemory = 0;
    private long tenSecondsAgoMemory = 0;

    private void Awake()
    {
        if (this.printMemoryLogs)
        {
            Debug.developerConsoleVisible = true;
            StartCoroutine(LogMemoryUse());
            Logger.Log("Incremental GC: " + GarbageCollector.isIncremental, this, LogType.WARNING);
        }
    }

    private IEnumerator LogMemoryUse()
    {
        int iteration = 0;
        while (true)
        {
            long memory = Process.GetCurrentProcess().PrivateMemorySize64 / (1024*1024);
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
