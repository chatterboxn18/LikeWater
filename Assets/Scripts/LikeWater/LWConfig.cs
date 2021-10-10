using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class LWConfig 
{
	public const string Timer = "PreferredTimer";
	public const string HasNotification = "Timer_HasNotification";
	public const string HasSound = "Timer_HasSound";
	public const string VolumeSetting = "Timer_Volume";
	public const string ClipIndex = "Timer_Clip";
	public const string StreakCounter = "Streak_Counter";
	public const string StreakDay = "Streak_Date";
	public const string FirstInstructions = "First_Instructions";
	public const string AttributeWaterKey = "water";
	public const string DataSaveName = "likewater";
	public const string CardDataSaveName = "likewater-cards";
	public const string PageIndexName = "PageIndex";
	public const string NotificationTitle = "Like Water";
	public const string WaterNotificationDescription = "Time to quench your thrist. Let's drink some water <3";
	public const string StreamNotificationDescription = "Let's make it like a river and stream Like Water.";
	public const string NotificationChannel = "like_water";
	public const string ServerKey = "Server_Key";
	public const string ServerPath = "https://BUCKET.amazonaws.com/LikeWater/";
	public const string USServer = "revelupgames.s3.us-west-1";
	public const string EUServer = "revelupgames-eu.s3.eu-west-2";
	public const string KRServer = "revelupgames-kr.s3.ap-northeast-2";
	public const string SEServer = "revelupgames-se.s3.ap-southeast-1";
	
	public const string ConfigFile = "likewater-config.json";
	public const string LastModifiedKey = "last-modified";
	public const float FadeTime = 0.2f;

	public const int CardWidth = 900;
	public const int CardHeight = 1200;

	public static Color SelectedColor = new Color(39,134,197, 1);
	public static Color MainColor = new Color(105,169,229, 1);
}
