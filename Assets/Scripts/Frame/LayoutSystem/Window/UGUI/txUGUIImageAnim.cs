﻿using System;
using System.Collections.Generic;
using UnityEngine;

public enum EFFECT_ALIGN
{
	NONE,			// 特效中心对齐父节点中心
	PARENT_BOTTOM,	// 特效底部对齐父节点底部
	POSITION_LIST,	// 使用位置列表对每一帧进行校正
}

public class txUGUIImageAnim : txUGUIImage, IUIAnimation
{
	protected List<TextureAnimCallBack> mPlayEndCallback;  // 一个序列播放完时的回调函数,只在非循环播放状态下有效
	protected List<TextureAnimCallBack> mPlayingCallback;  // 一个序列正在播放时的回调函数
	protected List<string> mTextureNameList;
	protected List<Vector2> mTexturePosList;
	protected AnimControl mControl;
	protected EFFECT_ALIGN mEffectAlign;
	protected string mTextureSetName;
	protected bool mUseTextureSize;
	public txUGUIImageAnim()
	{
		mControl = new AnimControl();
		mTextureNameList = new List<string>();
		mPlayEndCallback = new List<TextureAnimCallBack>();
		mPlayingCallback = new List<TextureAnimCallBack>();
		mUseTextureSize = false;
		mEffectAlign = EFFECT_ALIGN.NONE;
	}
	public override void init(GameObject go, txUIObject parent)
	{
		base.init(go, parent);
		string spriteName = getSpriteName();
		if(!isEmpty(spriteName))
		{
			int index = spriteName.LastIndexOf('_');
			if (index >= 0)
			{
				string textureSetName = spriteName.Substring(0, index);
				setTextureSet(textureSetName);
			}
		}
		mControl.setObject(this);
		mControl.setPlayEndCallback(onPlayEnd);
		mControl.setPlayingCallback(onPlaying);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mTextureNameList.Count == 0)
		{
			setSpriteName(null, false);
		}
		mControl.update(elapsedTime);
	}
	public override void setAtlas(Texture2D atlas, bool clearSprite = false)
	{
		if(atlas == getAtlas())
		{
			return;
		}
		// 改变图集时先停止播放
		stop();
		base.setAtlas(atlas, clearSprite);
		// 图集改变后清空当前序列帧列表
		setTextureSet(null);
	}
	public string getTextureSet() { return mTextureSetName; }
	public int getTextureFrameCount() { return mTextureNameList.Count; }
	public void setUseTextureSize(bool useSize){mUseTextureSize = useSize;}
	public void setTexturePosList(List<Vector2> posList) 
	{
		mTexturePosList = posList;
		if (mTexturePosList != null)
		{
			setEffectAlign(EFFECT_ALIGN.POSITION_LIST);
		}
	}
	public List<Vector2> getTexturePosList() { return mTexturePosList; }
	public void setEffectAlign(EFFECT_ALIGN align) { mEffectAlign = align; }
	public void setTextureSet(string textureSetName)
	{
		if (mTextureSetName != textureSetName)
		{
			setTextureSet(textureSetName, null);
		}
	}
	public void setTextureSet(string textureSetName, string subPath)
	{
		if(mTextureSetName == textureSetName)
		{
			return;
		}
		mTextureNameList.Clear();
		mTextureSetName = textureSetName;
		if (mAtlas != null && !isEmpty(mTextureSetName))
		{
			var sprites = mTPSpriteManager.getSprites(mAtlas);
			int index = 0;
			while(true)
			{
				string name = mTextureSetName + "_" + intToString(index++);
				if(sprites.ContainsKey(name))
				{
					mTextureNameList.Add(name);
				}
				else
				{
					break;
				}
			}
			if(getTextureFrameCount() == 0)
			{
				logError("invalid sprite anim! atlas : " + mAtlas.name + ", anim set : " + textureSetName);
			}
		}
		mControl.setFrameCount(getTextureFrameCount());
	}
	public LOOP_MODE getLoop() { return mControl.getLoop(); }
	public float getInterval() { return mControl.getInterval(); }
	public float getSpeed() { return mControl.getSpeed(); }
	public int getStartIndex() { return mControl.getStartIndex(); }
	public float getPlayedTime() { return mControl.getPlayedTime(); }
	public float getLength() { return mControl.getLength(); }
	public PLAY_STATE getPlayState() { return mControl.getPlayState(); }
	public bool getPlayDirection() { return mControl.getPlayDirection(); }
	public int getEndIndex() { return mControl.getEndIndex(); }
	public bool isAutoHide() { return mControl.isAutoResetIndex(); }
	// 获得实际的终止下标,如果是自动获得,则返回最后一张的下标
	public int getRealEndIndex() { return mControl.getRealEndIndex(); }
	public void setLoop(LOOP_MODE loop) { mControl.setLoop(loop); }
	public void setInterval(float interval) { mControl.setInterval(interval); }
	public void setSpeed(float speed) { mControl.setSpeed(speed); }
	public void setPlayDirection(bool direction) { mControl.setPlayDirection(direction); }
	public void setAutoHide(bool autoHide) { mControl.setAutoHide(autoHide); }
	public void setStartIndex(int startIndex) { mControl.setStartIndex(startIndex); }
	public void setEndIndex(int endIndex) { mControl.setEndIndex(endIndex); }
	public void stop(bool resetStartIndex = true, bool callback = true, bool isBreak = true) { mControl.stop(resetStartIndex, callback, isBreak); }
	public void play() { mControl.play(); }
	public void pause() { mControl.pause(); }
	public int getCurFrameIndex() { return mControl.getCurFrameIndex(); }
	public void setCurFrameIndex(int index) { mControl.setCurFrameIndex(index); }
	public void addPlayEndCallback(TextureAnimCallBack callback, bool clear = true)
	{
		if (clear)
		{
			List<TextureAnimCallBack> tempList = mListPool.newList(out tempList);
			tempList.AddRange(mPlayEndCallback);
			mPlayEndCallback.Clear();
			// 如果回调函数当前不为空,则是中断了更新
			foreach (var item in tempList)
			{
				item(this, true);
			}
			mListPool.destroyList(tempList);
		}
		if(callback != null)
		{
			mPlayEndCallback.Add(callback);
		}
	}
	public void addPlayingCallback(TextureAnimCallBack callback, bool clear = true)
	{
		if(clear)
		{
			mPlayingCallback.Clear();
		}
		if(callback != null)
		{
			mPlayingCallback.Add(callback);
		}
	}
	//--------------------------------------------------------------------------------------------------------
	protected void onPlaying(AnimControl control, int frame, bool isPlaying)
	{
		if(mControl.getCurFrameIndex() >= mTextureNameList.Count)
		{
			return;
		}
		setSpriteName(mTextureNameList[mControl.getCurFrameIndex()], mUseTextureSize);
		// 使用位置列表进行校正
		if (mEffectAlign == EFFECT_ALIGN.POSITION_LIST)
		{
			if (mTexturePosList != null && mTexturePosList.Count > 0)
			{
				int positionIndex = (int)(frame / (float)mTextureNameList.Count * mTexturePosList.Count + 0.5f);
				setPosition(mTexturePosList[positionIndex]);
			}
		}
		// 对齐父节点的底部
		else if(mEffectAlign == EFFECT_ALIGN.PARENT_BOTTOM)
		{
			txUIObject parent = getParent();
			if (parent != null)
			{
				Vector2 windowSize = getWindowSize();
				Vector2 parentSize = parent.getWindowSize();
				setPosition(replaceY(getPosition(), (windowSize.y - parentSize.y) * 0.5f));
			}
		}
		int count = mPlayingCallback.Count;
		for(int i = 0; i < count; ++i)
		{
			mPlayingCallback[i](this, false);
		}
	}
	protected void onPlayEnd(AnimControl control, bool callback, bool isBreak)
	{
		// 正常播放完毕后根据是否重置下标来判断是否自动隐藏
		if (!isBreak && mControl.isAutoResetIndex())
		{
			setActive(false);
		}
		if(callback)
		{
			List<TextureAnimCallBack> tempList = mListPool.newList(out tempList);
			tempList.AddRange(mPlayEndCallback);
			mPlayEndCallback.Clear();
			foreach (var item in tempList)
			{
				item(this, isBreak);
			}
			mListPool.destroyList(tempList);
		}
		else
		{
			mPlayEndCallback.Clear();
		}
	}
}