using System;
using System.Collections;
using System.Collections.Generic;
using LikeWater;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class LWNotificationManager : MonoBehaviour
{

	public enum NotificationType
	{
		Water = 0, 
		Stream = 1
	}
	
	private void Start()
	{
#if UNITY_ANDROID
		var channel = new AndroidNotificationChannel()
		{
			Id = LWConfig.NotificationChannel,
			Name = "Like Water Channel",
			Importance =  Importance.High, 
			Description = "Channel for Like Water App"
		};
		AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
	}
#if UNITY_ANDROID  
	public static AndroidNotification CreateNotification(NotificationType type, DateTime time, string inputData = "")
	{
		var title = "Like Water Reminder";
		var message = "";
		switch (type)
		{
			case NotificationType.Water:
				message = LWConfig.WaterNotificationDescription;
				break;
			case NotificationType.Stream:
				message = LWConfig.StreamNotificationDescription;
				break;
			default:
				message = LWConfig.WaterNotificationDescription;
				break;
		}
		var notification = new AndroidNotification
		{
			Title =  title, 
			Text = message, 
			FireTime = time, 
			IntentData = inputData,
			SmallIcon = "icon_0", 
			ShouldAutoCancel = true,
			RepeatInterval = TimeSpan.FromDays(7),
			Color = new Color(105,170,228)
		};
		return notification;
	}
#endif
}
