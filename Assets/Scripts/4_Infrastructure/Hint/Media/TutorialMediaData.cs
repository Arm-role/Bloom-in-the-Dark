#nullable enable

using UnityEngine;

// Abstract base SO สำหรับ media — concrete subclass: SpriteMediaData, VideoMediaData
// HintEntry._mediaData อ้างไปยัง subclass ใดก็ได้ผ่าน polymorphism
public abstract class TutorialMediaData : ScriptableObject, ITutorialMedia { }