﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraDebug : MonoBehaviour
{
	protected GameCamera mGameCamera;
	public string CurLinkerName;
	public string LinkedObjectName;
	public GameObject LinkedObject;
	public Vector3 Relative;
	public Vector3 CurRelative;
	public List<string> ActiveComponent = new List<string>();
	public void Update()
	{
		if (mGameCamera == null)
		{
			return;
		}
		CameraLinker linker = mGameCamera.getCurLinker();
		if (linker != null)
		{
			CurLinkerName = linker.GetType().ToString();
			LinkedObject = linker.getLinkObject().getObject();
			LinkedObjectName = linker.getLinkObject().getName();
			Relative = linker.getRelativePosition();
			if (LinkedObject != null)
			{
				CurRelative = mGameCamera.getWorldPosition() - LinkedObject.transform.position;
			}
			else
			{
				CurRelative = Vector3.zero;
			}
		}
		else
		{
			CurLinkerName = StringUtility.EMPTY_STRING;
			LinkedObjectName = StringUtility.EMPTY_STRING;
			LinkedObject = null;
			Relative = Vector3.zero;
			CurRelative = Vector3.zero;
		}
		ActiveComponent.Clear();
		var allComponents = mGameCamera.getAllComponent();
		foreach (var item in allComponents)
		{
			if (item.Value.isActive())
			{
				ActiveComponent.Add(item.Key.ToString());
			}
		}
	}
	public void setCamera(GameCamera camera)
	{
		mGameCamera = camera;
	}
	//-------------------------------------------------------------------------------------------------------
}