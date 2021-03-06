using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AnchorMenu
{
	public const string mAutoAnchorMenuName = "GameAnchor/";
	public const string mPaddingAnchorMenuName = "PaddingAnchor/";
	public const string mScaleAnchorMenuName = "ScaleAnchor/";
	[MenuItem(mAutoAnchorMenuName + "Start PreviewAnchor %,")]
	public static void startPreviewLayoutAnchor()
	{
		if(Selection.activeGameObject == null)
		{
			Debug.Log("需要选中场景结构面板中的节点");
			return;
		}
		Vector2 gameViewSize = ReflectionUtility.getGameViewSize();
		string info = "是否要预览" + Selection.activeGameObject.name + "在" + gameViewSize.x + "x" + gameViewSize.y + "下的显示?";
		if(!EditorUtility.DisplayDialog("预览", info, "确定", "取消"))
		{
			return;
		}
		// 设置摄像机的Z坐标为视图高的一半,设置画布根节点为屏幕大小
		if (ReflectionUtility.isNGUI(Selection.activeGameObject))
		{
#if USE_NGUI
			GameObject nguiRootObject = ReflectionUtility.getGameObject(null, GameDefine.NGUI_ROOT);
			UIRoot nguiRoot = nguiRootObject.GetComponent<UIRoot>();
			nguiRoot.scalingStyle = UIRoot.Scaling.Constrained;
			nguiRoot.manualWidth = (int)gameViewSize.x;
			nguiRoot.manualHeight = (int)gameViewSize.y;
			GameObject camera = ReflectionUtility.getGameObject(nguiRootObject, ReflectionUtility.getUICameraName(), true);
			camera.transform.localPosition = new Vector3(0.0f, 0.0f, -gameViewSize.y * 0.5f);
#endif
		}
		else
		{
			GameObject uguiRootObj = ReflectionUtility.getGameObject(null, GameDefine.UGUI_ROOT);
			RectTransform transform = uguiRootObj.GetComponent<RectTransform>();
			transform.offsetMin = new Vector2(-gameViewSize.x * 0.5f, -gameViewSize.y * 0.5f);
			transform.offsetMax = new Vector2(gameViewSize.x * 0.5f, gameViewSize.y * 0.5f);
			transform.anchorMax = Vector2.zero;
			transform.anchorMin = Vector2.zero;
			GameObject camera = ReflectionUtility.getGameObject(uguiRootObj, ReflectionUtility.getUICameraName(), true);
			camera.transform.localPosition = new Vector3(0.0f, 0.0f, -gameViewSize.y * 0.5f);
		}
		ReflectionUtility.applyAnchor(Selection.activeGameObject, true);
	}
	[MenuItem(mAutoAnchorMenuName + "End PreviewAnchor %.")]
	public static void endPreviewLayoutAnchor()
	{
		// 恢复摄像机设置
#if USE_NGUI
		GameObject nguiRootObject = ReflectionUtility.getGameObject(null, GameDefine.NGUI_ROOT);
		UIRoot nguiRoot = nguiRootObject.GetComponent<UIRoot>();
		nguiRoot.scalingStyle = UIRoot.Scaling.Constrained;
		nguiRoot.manualWidth = ReflectionUtility.getStandardWidth();
		nguiRoot.manualHeight = ReflectionUtility.getStandardHeight();
		GameObject nguiCamera = ReflectionUtility.getGameObject(nguiRootObject, ReflectionUtility.getUICameraName(), true);
		nguiCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -ReflectionUtility.getStandardHeight() * 0.5f);
#endif
		GameObject uguiRootObj = ReflectionUtility.getGameObject(null, GameDefine.UGUI_ROOT);
		RectTransform transform = uguiRootObj.GetComponent<RectTransform>();
		transform.offsetMin = new Vector2(-ReflectionUtility.getStandardWidth() * 0.5f, -ReflectionUtility.getStandardHeight() * 0.5f);
		transform.offsetMax = new Vector2(ReflectionUtility.getStandardWidth() * 0.5f, ReflectionUtility.getStandardHeight() * 0.5f);
		transform.anchorMax = Vector2.zero;
		transform.anchorMin = Vector2.zero;
		GameObject uguiCamera = ReflectionUtility.getGameObject(uguiRootObj, ReflectionUtility.getUICameraName(), true);
		uguiCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -ReflectionUtility.getStandardHeight() * 0.5f);
	}
	[MenuItem(mAutoAnchorMenuName + mPaddingAnchorMenuName + "AddAnchor")]
	public static void addPaddingAnchor()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// 所选择的物体必须在同一个父节点下
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				ReflectionUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 0; i < count; ++i)
		{
			addPaddingAnchor(Selection.gameObjects[i]);
		}
	}
	[MenuItem(mAutoAnchorMenuName + mPaddingAnchorMenuName + "RemoveAnchor")]
	public static void removePaddingAnchor()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// 所选择的物体必须在同一个父节点下
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				ReflectionUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 0; i < count; ++i)
		{
			removePaddingAnchor(Selection.gameObjects[i]);
		}
	}
	[MenuItem(mAutoAnchorMenuName + mScaleAnchorMenuName + "AddAnchorKeepAspect &1")]
	public static void addScaleAnchorKeepAspect()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// 所选择的物体必须在同一个父节点下
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				ReflectionUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 0; i < count; ++i)
		{
			addScaleAnchor(Selection.gameObjects[i], true);
		}
	}
	[MenuItem(mAutoAnchorMenuName + mScaleAnchorMenuName + "SetRaycastTargetFalse &3")]
	public static void setRaycastTargetFalse()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// 所选择的物体必须在同一个父节点下
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				ReflectionUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 0; i < count; ++i)
		{
			setSeleteRaycastTarget(Selection.gameObjects[i], false);
		}
	}
	public static void setSeleteRaycastTarget(GameObject obj, bool target)
	{
		// 先设置自己的Anchor
		if(obj.GetComponent<Graphic>() != null)
		{
			obj.GetComponent<Graphic>().raycastTarget = target;
		}
		// 再设置子节点的Anchor
		int childCount = obj.transform.childCount;
		for(int i = 0; i < childCount; ++i)
		{
			setSeleteRaycastTarget(obj.transform.GetChild(i).gameObject, target);
		}
	}
	[MenuItem(mAutoAnchorMenuName + mScaleAnchorMenuName + "AddAnchor")]
	public static void addScaleAnchor()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// 所选择的物体必须在同一个父节点下
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				ReflectionUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 0; i < count; ++i)
		{
			addScaleAnchor(Selection.gameObjects[i], false);
		}
	}
	[MenuItem(mAutoAnchorMenuName + mScaleAnchorMenuName + "RemoveAnchor &2")]
	public static void removeScaleAnchor()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// 所选择的物体必须在同一个父节点下
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				ReflectionUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 0; i < count; ++i)
		{
			removeScaleAnchor(Selection.gameObjects[i]);
		}
	}
	//-------------------------------------------------------------------------------------------------------------------
	public static void addPaddingAnchor(GameObject obj)
	{
		// 先设置自己的Anchor
		if (obj.GetComponent<PaddingAnchor>() == null)
		{
			// 只要有Rect就可以添加该组件,panel也可以添加
			if(ReflectionUtility.isNGUI(obj))
			{
#if USE_NGUI
				if (obj.GetComponent<UIRect>() != null)
				{
					obj.AddComponent<PaddingAnchor>().setAnchorMode(ANCHOR_MODE.AM_NEAR_PARENT_SIDE);
				}
#endif
			}
			else
			{
				obj.AddComponent<PaddingAnchor>().setAnchorMode(ANCHOR_MODE.AM_NEAR_PARENT_SIDE);
			}
		}
		// 再设置子节点的Anchor
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			addPaddingAnchor(obj.transform.GetChild(i).gameObject);
		}
	}
	public static void removePaddingAnchor(GameObject obj)
	{
		// 先销毁自己的Anchor
		if (obj.GetComponent<PaddingAnchor>() != null)
		{
			ReflectionUtility.destroyGameObject(obj.GetComponent<PaddingAnchor>(), true);
		}
		// 再销毁子节点的Anchor
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			removePaddingAnchor(obj.transform.GetChild(i).gameObject);
		}
	}
	public static void addScaleAnchor(GameObject obj, bool keepAspect)
	{
		// 先设置自己的Anchor
		if (obj.GetComponent<ScaleAnchor>() == null)
		{
			obj.AddComponent<ScaleAnchor>();
		}
		obj.GetComponent<ScaleAnchor>().mKeepAspect = keepAspect;
		// 再设置子节点的Anchor
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			addScaleAnchor(obj.transform.GetChild(i).gameObject, keepAspect);
		}
	}
	public static void removeScaleAnchor(GameObject obj)
	{
		// 先销毁自己的Anchor
		if (obj.GetComponent<ScaleAnchor>() != null)
		{
			ReflectionUtility.destroyGameObject(obj.GetComponent<ScaleAnchor>(), true);
		}
		// 再销毁子节点的Anchor
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			removeScaleAnchor(obj.transform.GetChild(i).gameObject);
		}
	}
}