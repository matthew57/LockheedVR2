using UnityEngine;
using System.Collections;
using System;

public class TimeConverter  {

	public static string getClockTime(float seconds)
	{
		TimeSpan s = TimeSpan.FromSeconds(seconds);

		string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", 
			s.Hours, 
			s.Minutes, 
			s.Seconds);
		return answer;
	}
	

}
