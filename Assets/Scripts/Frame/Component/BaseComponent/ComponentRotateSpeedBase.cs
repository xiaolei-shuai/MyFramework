﻿using System;
using UnityEngine;
using System.Collections;

public class ComponentRotateSpeedBase : GameComponent, IComponentModifyRotation, IComponentBreakable
{
	public PLAY_STATE mPlayState;
	public Vector3 mRotateSpeed;				// 欧拉角旋转速度
	public Vector3 mRotateAcceleration;			// 旋转加速度
	public Vector3 mCurRotation;
	public ComponentRotateSpeedBase()
	{
		mPlayState = PLAY_STATE.PS_STOP;
	}
	public Vector3 getRotateSpeed() { return mRotateSpeed; }
	public Vector3 getRotateAcceleration() { return mRotateAcceleration; }
	public void setRotateSpeed(Vector3 speed) { mRotateSpeed = speed; }
	public void setRotateAcceleration(Vector3 acceleration) { mRotateAcceleration = acceleration; }
	public void startRotateSpeed(Vector3 startAngle, Vector3 rotateSpeed, Vector3 rotateAcceleration) 
	{
		pause(false);
		mCurRotation = startAngle;
		mRotateSpeed = rotateSpeed;
		mRotateAcceleration = rotateAcceleration;
		applyRotation(ref mCurRotation, false, true);
		// 如果速度和加速度都为0,则停止旋转
		if (isVectorZero(ref rotateSpeed) && isVectorZero(ref rotateAcceleration))
		{
			setActive(false);
		}
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if (!active)
		{
			stop();
		}
	}
	public virtual void stop() { mPlayState = PLAY_STATE.PS_STOP; }
	public void pause(bool pause) { mPlayState = pause ? PLAY_STATE.PS_PAUSE : PLAY_STATE.PS_PLAY; }
	public void setPlayState(PLAY_STATE state)
	{
		if (mComponentOwner == null)
		{
			return;
		}
		if (state == PLAY_STATE.PS_PLAY)
		{
			pause(false);
		}
		else if (state == PLAY_STATE.PS_PAUSE)
		{
			pause(true);
		}
		else if (state == PLAY_STATE.PS_STOP)
		{
			stop();
		}
	}
	public PLAY_STATE getPlayState() { return mPlayState; }
	public void notifyBreak(){}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected virtual void applyRotation(ref Vector3 rotation, bool done = false, bool refreshNow = false) { }
	protected virtual Vector3 getCurRotation() { return Vector3.zero; }
}