using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class LWConfig 
{
	public const string Timer = "PreferredTimer";
	public const string HasNotification = "Timer_HasNotification";
	public const string HasSound = "Timer_HasSound";
	public const string AttributeWaterKey = "water";
	public const string DataSaveName = "likewater";
	public const string PageIndexName = "PageIndex";
	public const string NotificationTitle = "Like Water";
	public const string WaterNotificationDescription = "Time to quench your thrist. Let's drink some water <3";
	public const string StreamNotificationDescription = "Let's make it like a river and stream like water.";
	public const string NotificationChannel = "like_water";
	public const string ServerPath = "https://revelupgames.s3-us-west-1.amazonaws.com/LikeWater/";
	public const string ConfigFile = "likewater-config.json";
	public const string LastModifiedKey = "last-modified";
	public const float FadeTime = 0.2f;
	
	

	public static Color SelectedColor = new Color(39,134,197, 1);
	public static Color MainColor = new Color(105,169,229, 1);
}
