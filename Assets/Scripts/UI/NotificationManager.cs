//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationManager : SingletonBehavior<NotificationManager>
{
    [SerializeField] private GameObject notificationPrefab;

    public void CreateNotification(string content, float duration) {
        Logger.Log("Creating new notification", this, LogType.INFO);
        Notification newNotification = Instantiate(notificationPrefab, this.transform.GetChild(0)).GetComponent<Notification>();
        newNotification.Initiate(content, duration);
    }

    public void CreateNotification(string content, Func<bool> predicate) {
        Notification newNotification = Instantiate(notificationPrefab, this.transform.GetChild(0)).GetComponent<Notification>();
        newNotification.Initiate(content, predicate);
    }
}
