using UnityEngine;
using ClemCAddons;
using System.Collections.Generic;

public class SplineDecorator : MonoBehaviour {

	public BezierSpline spline;

	public int StepSize;

	public bool lookForward;

	public Transform[] items;

	public bool spawnOnPlay;

	public bool spawnSelectedOnly;

	public void SpawnNow()
    {
		if (StepSize <= 0 || items == null || items.Length == 0)
		{
			return;
		}
		float stepSize = StepSize * items.Length;
		if (spline.Loop || stepSize == 1)
		{
			stepSize = 1f / stepSize;
		}
		else
		{
			stepSize = 1f / (stepSize - 1);
		}
		if (spawnSelectedOnly)
        {
			List<int> toSpawn = new List<int>();
			for (int t = 0; t < spline.CurveCount; t++)
			{
				if (spline.curves[t].Spawn || !spawnSelectedOnly)
				{
					toSpawn.Add(t);
					stepSize += spline.GetCurve(t).Frequency;
				}
			}
			for (int e = 0; e < spline.CurveCount; e++)
			{
				stepSize = items.Length * spline.GetCurve(e).Frequency;
				if (spline.Loop || stepSize == 1)
				{
					stepSize = 1f / stepSize;
				}
				else
				{
					stepSize = 1f / (stepSize - 1);
				}
				for (int p = 0, f = 0; f < spline.GetCurve(e).Frequency; f++)
				{
					for (int i = 0; i < items.Length; i++, p++)
					{
						Vector3 position = spline.GetPointInCurve(p * stepSize * (1 - (stepSize*0.8f)), e, toSpawn.ToArray());
						if (!position.IsInfinite())
						{
							Transform item = Instantiate(items[i]) as Transform;
							item.transform.localPosition = position;
							if (lookForward)
							{
								item.transform.LookAt(position + spline.GetDirectionInCurve(p * stepSize, e));
							}
							item.transform.parent = transform;
						}
					}
				}
			}
		} else
        {
			for (int p = 0, f = 0; f < StepSize; f++)
			{
				for (int i = 0; i < items.Length; i++, p++)
				{
					Vector3 position = spline.GetPoint(p * stepSize);
					Transform item = Instantiate(items[i]) as Transform;
					item.transform.localPosition = position;
					if (lookForward)
					{
						item.transform.LookAt(position + spline.GetDirection(p * stepSize));
					}
					item.transform.parent = transform;
				}
			}
		}
	}
	public void Clear()
	{
		for(int i = transform.childCount-1; i >= 0; i--)
        {
			DestroyImmediate(transform.GetChild(i).gameObject);
        }
	}

	private void Awake () {
		if(spawnOnPlay)
			SpawnNow();
	}
}