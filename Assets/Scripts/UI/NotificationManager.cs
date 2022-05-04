//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : SingletonBehavior<NotificationManager>
{
    [SerializeField] private GameObject notificationPrefab;

    public Notification CreateNotification(string content, float duration) {
        Logger.Log("Creating new notification", this, LogType.INFO);
        Notification newNotification = Instantiate(notificationPrefab, this.transform.GetChild(0)).GetComponent<Notification>();
        return newNotification.Initiate(content, duration);
    }
    
    public Notification CreateNotification(string content) {
        Logger.Log("Creating new notification", this, LogType.INFO);
        Notification newNotification = Instantiate(notificationPrefab, this.transform.GetChild(0)).GetComponent<Notification>();
        return newNotification.Initiate(content);
    }
}
