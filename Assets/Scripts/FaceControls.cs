using UnityEngine;
using System.Collections;
using System.IO;

public class FaceControls : MonoBehaviour
{	
	public GameObject prefab;
	public Transform lastFace;
	
	private int automate_NumScreenshots = 0;
	private float automate_DelayPerScreenshot = 0.02f;
	private string automate_ScreenshotsDirectory = "";

	private bool automated = false;

	private string basePath
	{
		get
		{
			return Application.dataPath + "/../";
		}
	}

	void Start()
	{
		var config = IniFile.LoadFile("config.ini");

		if(config != null)
		{
			automated = (bool)config["Automate"]["Enabled"];
			if(automated)
			{
				automate_ScreenshotsDirectory = (string)config["Automate"]["ScreenshotsDirectory"];
				automate_NumScreenshots = (int)config["Automate"]["NumScreenshots"];
				automate_DelayPerScreenshot = (float)config["Automate"]["DelayPerScreenshot"];

				int width  = (int)config["Automate"]["Width"];
				int height = (int)config["Automate"]["Height"];

				Screen.SetResolution(width, height, false);
			}
		}

		if(automate_NumScreenshots > 0)
		{
			StartCoroutine(AutomaticScreenshots());
		}
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.X))
		{
			NextFace();
		}

		if(Input.GetKeyDown(KeyCode.Z))
		{
			Screenshot();
		}
	}

	IEnumerator AutomaticScreenshots()
	{
		// wait a 'long' time so that the unity black-in fades away fully
		yield return new WaitForSeconds(0.5f);

		while(automate_NumScreenshots > 0)
		{
			NextFace();

			yield return new WaitForSeconds(automate_DelayPerScreenshot);

			Screenshot();
			automate_NumScreenshots -= 1;
			
			yield return new WaitForSeconds(automate_DelayPerScreenshot);
		}

		Debug.Log("AutomaticScreenshots: done!");
		Application.Quit();
	}

	void NextFace()
	{
		if(lastFace != null)
		{
			Destroy(lastFace.gameObject);
		}
		
		var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
		obj.SendMessage("Randomize");
		lastFace = obj.transform;
	}

	void Screenshot()
	{
		string[] existingScreenshots = Directory.GetFiles(basePath + automate_ScreenshotsDirectory, "*.png", SearchOption.TopDirectoryOnly);
		int screenshotNum = existingScreenshots.Length + 1;

		string name = basePath + automate_ScreenshotsDirectory + string.Format("face_{0}.png", screenshotNum);

		Application.CaptureScreenshot(name);
		
		Debug.Log("Created screenshot: " + name);
	}
}
