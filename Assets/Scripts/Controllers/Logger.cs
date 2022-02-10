using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Enum for types of logging messages
public enum LogType {ERROR, WARNING, INFO, DEBUG}

// Class for a program wider logger that takes into account different levels of logs
public class Logger : SingletonBehavior<Logger>
{
    // Level of logging currently being displayed in Unity
    [SerializeField]
    private LogType loggingLevel;
    
    // Some frequently used log messages
    public static string SINGLETON_VIOLATION = "Singleton pattern violated.";
    public static string TEST = "Test.";

    // Static method to print a log
    public static void Log(string message, System.Object caller, LogType logType = LogType.DEBUG) {
        Logger.Singleton.SendLog(message, caller, logType);
    }

    // Method to print a log
    private void SendLog(string message, System.Object caller, LogType logType = LogType.DEBUG) {
        Type callerType = caller.GetType();

        if (logType <= this.loggingLevel) {
            switch (logType) {
                case LogType.ERROR:
                Debug.LogError(String.Format("[{0}] {1}", callerType.ToString(), message));
                Console.WriteLine(String.Format("[{0}] {1}", callerType.ToString(), message));
                break;
                case LogType.WARNING:
                Debug.LogWarning(String.Format("[{0}] {1}", callerType.ToString(), message));
                Console.WriteLine(String.Format("[{0}] {1}", callerType.ToString(), message));
                break;
                default: 
                Debug.Log(String.Format("[{0}] {1}", callerType.ToString(), message));
                Console.WriteLine(String.Format("[{0}] {1}", callerType.ToString(), message));
                break;
            }
        }
    }
}
