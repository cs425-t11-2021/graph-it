using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationManager : SingletonBehavior<NotificationManager>
{
    [SerializeField] private GameObject notificationPrefab;

    public void CreateNotification(string content, float duration) {
        // Logger.Log("Creating new notification", this, LogType.INFO);
        // Notification newNotification = Instantiate(notificationPrefab, this.transform).GetComponent<Notification>();
        // newNotification.Initiate(content, duration);
    }

    public void CreateNotification(string content, Func<bool> predicate) {
        // Notification newNotification = Instantiate(notificationPrefab, this.transform).GetComponent<Notification>();
        // newNotification.Initiate(content, predicate);
    }
}
