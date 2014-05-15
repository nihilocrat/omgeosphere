using UnityEngine;
using System.Collections;

public class FaceMaker : MonoBehaviour
{
	public Transform bg;
	public Transform face;
	public Transform[] eyes;
	public LineRenderer mouth;
	public LineRenderer mouth_straight;

	public Color[] bgColors;
	public Material[] faceColors;
	
	public float scaleStretch = 1f;
	public Vector3 scaleRandomness = Vector3.zero;
	public Vector3 orientationRandomness = Vector3.zero;
	public float eyeSpacing = 1f;
	public float eyeSkew = 1f;
	public float eyeElevation = 1f;
	public float eyebrowTilt = 0.5f;
	public Vector3 eyeRotation = Vector3.zero;
	public float eyeNoWhiteChance = 0.2f;
	public int mouthPoints = 9;
	public float mouthSquigglyness = 0f;
	public Vector2 mouthSize = Vector2.one;
	public float mouthElevation = 1f;
	public float mouthSmileChance = 0.2f;
	public float mouthOpenChance = 0.2f;
	public float mouthInvertChance = 0.2f;

	void Randomize()
	{
		RandomFace();
		RandomEyes();
		RandomMouth();

		// quick an dertay
		bg = GameObject.Find("bg").transform;

		Color bgColor = bgColors[Random.Range(0, bgColors.Length)];
		bg.renderer.sharedMaterial.color = bgColor;
	}

	void RandomFace()
	{
		Vector3 rotVec = orientationRandomness;

		rotVec.x *= (Random.value - 0.5f);
		rotVec.y *= (Random.value - 0.5f);
		rotVec.z *= (Random.value - 0.5f);

		transform.Rotate(rotVec, Space.Self);

		Material faceMat = faceColors[Random.Range(0, faceColors.Length)];
		face.renderer.sharedMaterial = faceMat;

		Vector3 scaleVec = Vector3.one;

		var scaleRandom = Random.value * scaleStretch;
		switch(Random.Range(1,3))
		{
			case 1:
				scaleVec.x += scaleRandom;
				break;
			case 2:
				scaleVec.y += scaleRandom;
				break;
			case 3:
				scaleVec.z += scaleRandom;
				break;
		}

		/*
		scaleVec.x += (Random.value - 0.5f) * scaleRandomness.x;
		scaleVec.y += (Random.value - 0.5f) * scaleRandomness.y;
		scaleVec.z += (Random.value - 0.5f) * scaleRandomness.z;
		*/

		face.localScale = scaleVec;
	}

	void RandomEyes()
	{
		var eyeParent = eyes[0].parent;
		eyeParent.localPosition += (Vector3.up * (Random.value - 0.5f) * eyeElevation);
		
		bool noWhite = (Random.value <= eyeNoWhiteChance);

		foreach(Transform i in eyes)
		{
			// set up eyebrow
			LineRenderer eyebrow = i.GetComponentInChildren<LineRenderer>();

			int vertex =  Random.Range(0, 1);
			var eyebrowPos = new Vector3(0.5f, (Random.value - 0.5f) * eyebrowTilt, 0f);
			if(vertex == 0)
			{
				eyebrowPos.x *= -1f;
			}
			eyebrow.SetPosition(vertex, eyebrowPos);
			// end eyebrow

			var pos = i.localPosition;

			pos.x += (Random.value - 0.5f) * eyeSpacing;
			pos.y += (Random.value - 0.5f) * eyeSkew;

			i.localPosition = pos;

			Vector3 rotVec = eyeRotation;
			
			rotVec.x *= (Random.value - 0.5f);
			rotVec.y *= (Random.value - 0.5f);
			rotVec.z *= (Random.value - 0.5f);
			i.Rotate(eyeRotation, Space.Self);

			if(noWhite)
			{
				i.renderer.enabled = false;
			}
		}
	}

	void RandomMouth()
	{
		float minX = -0.1f;
		float maxX = 0.1f;

		mouth.SetVertexCount(mouthPoints);

		bool smile = (Random.value <= mouthSmileChance);
		bool open = (Random.value <= mouthOpenChance);
		bool invert = (Random.value <= mouthInvertChance);

		mouth_straight.enabled = open;

		for(int i = 0; i < mouthPoints; i++)
		{
			Vector3 pos = Vector3.zero;
			pos.x = Mathf.Lerp(minX, maxX, (float)i / (float)mouthPoints);
			pos.y = (Random.value - 0.5f) * mouthSquigglyness;

			if(smile && i == 0 || i == mouthPoints-1)
			{
				pos.y += 0.05f;
			}

			if(open)
			{
				if(i == 0)
				{
					mouth_straight.SetPosition(0, pos);
				}
				else if(i == mouthPoints-1)
				{
					mouth_straight.SetPosition(1, pos);
				}
			}

			mouth.SetPosition(i, pos);
		}
		
		mouth.transform.localPosition += (Vector3.up * (Random.value - 0.5f) * mouthElevation);
		mouth.transform.localScale += Vector3.right * Mathf.Lerp(mouthSize.x, mouthSize.y, Random.value);
		
		if(invert)
		{
			mouth.transform.Rotate(0f, 0f, 180f, Space.Self);
		}
	}
}
