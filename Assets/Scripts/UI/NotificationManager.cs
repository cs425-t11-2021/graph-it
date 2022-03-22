using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationManager : SingletonBehavior<NotificationManager>
{
    [SerializeField] private GameObject notificationPrefab;

    public void CreateNoficiation(string content, float duration) {
        Notification newNotification = Instantiate(notificationPrefab, this.transform).GetComponent<Notification>();
        newNotification.Initiate(content, duration);
    }

    public void CreateNoficiation(string content, Func<bool> predicate) {
        Notification newNotification = Instantiate(notificationPrefab, this.transform).GetComponent<Notification>();
        newNotification.Initiate(content, predicate);
    }
}
