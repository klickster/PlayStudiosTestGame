using System.Collections;
using UnityEngine;

public class CoroutineProxy
{
	private MonoBehaviour monoBehaviour;

	public CoroutineProxy(MonoBehaviour monoBehaviour)
	{
		this.monoBehaviour = monoBehaviour;
	}

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		return monoBehaviour.StartCoroutine(routine);
	}

	public void StopCoroutine(Coroutine routine)
	{
		monoBehaviour.StopCoroutine(routine);
	}
}
