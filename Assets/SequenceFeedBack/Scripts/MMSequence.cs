using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//序列可能存在的状态
public enum MMSequenceTrackStates { Idle, Down, Up }

/// <summary>
/// 轨道片段的数据
/// </summary>
[Serializable]
public class MMSequenceNote
{
    //播放时间点
    public float Timestamp;
    //id默认-1
    public int ID;

    public virtual MMSequenceNote Copy()
    {
        MMSequenceNote newNote = new MMSequenceNote();
        newNote.ID = this.ID;
        newNote.Timestamp = this.Timestamp;
        return newNote;
    }
}
/// <summary>
/// 轨道数据包括颜色，状态
/// </summary>
[System.Serializable]
public class MMSequenceTrack
{
    public int ID = 0;
    public Color TrackColor;
    public KeyCode Key = KeyCode.Space;
    public bool Active = true;
    [MMFReadOnly]
    public MMSequenceTrackStates State = MMSequenceTrackStates.Idle;
    [HideInInspector]
    public bool Initialized = false;
        
    public virtual void SetDefaults(int index)
    {
        if (!Initialized)
        {
            ID = index;
           // TrackColor = MMSequence.RandomSequenceColor();
            Key = KeyCode.Space;
            Active = true;
            State = MMSequenceTrackStates.Idle;
            Initialized = true;
        }            
    }
}

/// <summary>
/// 序列列表
/// </summary>
[System.Serializable]
public class MMSequenceList
{
    public List<MMSequenceNote> Line;
}

//序列的储存文件
[CreateAssetMenu(menuName = "MoreMountains/Sequencer/MMSequence")]
public class MMSequencer : ScriptableObject
{
   [Header("Sequence")]
		/// 每个轨道时长
		[Tooltip("the length (in seconds) of the sequence")]
		[MMFReadOnly]
		public float Length;
		/// 原始序列(由输入序列记录器输出)
		[Tooltip("the original sequence (as outputted by the input sequence recorder)")]
		public MMSequenceList OriginalSequence;
		/// 自动播放下一个时间间隔
		[Tooltip("the duration in seconds to apply after the last input")]
		public float EndSilenceDuration = 0f;

		[Header("Sequence Contents")]
		/// 轨道实体列表
		[Tooltip("the list of tracks for this sequence")]
		public List<MMSequenceTrack> SequenceTracks;

		[Header("Quantizing")]
		/// 这个序列是否应该以量子化的形式使用
		[Tooltip("whether this sequence should be used in quantized form or not")]
		public bool Quantized;
		/// 序列流程
		[Tooltip("the target BPM for this sequence")]
		public int TargetBPM = 120;
		/// 轨道数据的列表长度（列）经过后期输入的
		[Tooltip("the contents of the quantized sequence")]
		public List<MMSequenceList> QuantizedSequence;
        //定义随机按钮颜色
		[Space]
		[Header("Controls")]
		[MMFInspectorButton("RandomizeTrackColors")]
		public bool RandomizeTrackColorsButton;
        //中间变量
		protected float[] _quantizedBeats; 
		protected List<MMSequenceNote> _deleteList;

		/// <summary>
		/// 比较两个序列数据的大小
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		static int SortByTimestamp(MMSequenceNote p1, MMSequenceNote p2)
		{
			return p1.Timestamp.CompareTo(p2.Timestamp);
		}

		/// <summary>
		/// 排序原始序列
		/// </summary>
		public virtual void SortOriginalSequence()
		{
			OriginalSequence.Line.Sort(SortByTimestamp);
		}

		/// <summary>
		/// 量化原始序列，填充QuantizedSequence列表，按节拍安排事件
		/// </summary>
		public virtual void QuantizeOriginalSequence()
		{
			ComputeLength();
			QuantizeSequenceToBPM(OriginalSequence.Line);
		}

		/// <summary>
		/// 获得序列的长度
		/// </summary>
		public virtual void ComputeLength()
		{
			Length = OriginalSequence.Line[OriginalSequence.Line.Count - 1].Timestamp + EndSilenceDuration;
		}

		/// <summary>
		/// 将 timestamp 映射和BMP关联
		/// </summary>
		public virtual void QuantizeSequenceToBPM(List<MMSequenceNote> baseSequence)
		{
			float sequenceLength = Length;
			float beatDuration = 60f / TargetBPM;
			//总时长/间隔
			int numberOfBeatsInSequence = (int)(sequenceLength / beatDuration);
			QuantizedSequence = new List<MMSequenceList>();
			_deleteList = new List<MMSequenceNote>();
			_deleteList.Clear();

			// we fill the BPM track with the computed timestamps
			_quantizedBeats = new float[numberOfBeatsInSequence];
			for (int i = 0; i < numberOfBeatsInSequence; i++)
			{
				_quantizedBeats[i] = i * beatDuration;
			}
            
			for (int i = 0; i < SequenceTracks.Count; i++)
			{
				QuantizedSequence.Add(new MMSequenceList());
				QuantizedSequence[i].Line = new List<MMSequenceNote>();
				//为每个轨道添加数据
				for (int j = 0; j < numberOfBeatsInSequence; j++)
				{
					MMSequenceNote newNote = new MMSequenceNote();
					newNote.ID = -1;
					newNote.Timestamp = _quantizedBeats[j];
					QuantizedSequence[i].Line.Add(newNote);
					//将原始(OriginalSequence)录制好的序列合并进QuantizedSequence
					foreach (MMSequenceNote note in baseSequence)
					{
						float newTimestamp = RoundFloatToArray(note.Timestamp, _quantizedBeats);
						if ((newTimestamp == _quantizedBeats[j]) && (note.ID == SequenceTracks[i].ID))
						{
							QuantizedSequence[i].Line[j].ID = note.ID;
						}
					}
				}
			}        
		}

		/// <summary>
		/// 初始化所有轨道数据
		/// </summary>
		protected virtual void OnValidate()
		{
			for (int i = 0; i < SequenceTracks.Count; i++)
			{
				SequenceTracks[i].SetDefaults(i);
			}
		}

		/// <summary>
		/// 随机轨道颜色
		/// </summary>
		protected virtual void RandomizeTrackColors()
		{
			foreach(MMSequenceTrack track in SequenceTracks)
			{
				track.TrackColor = RandomSequenceColor();
			}
		}

		/// <summary>
		/// 获得一个随机颜色
		/// </summary>
		/// <returns></returns>
		public static Color RandomSequenceColor()
		{
			int random = UnityEngine.Random.Range(0, 32);
			switch (random)
			{
				case 0: return new Color32(240, 248, 255, 255); 
				case 1: return new Color32(127, 255, 212, 255);
				case 2: return new Color32(245, 245, 220, 255);
				case 3: return new Color32(95, 158, 160, 255);
				case 4: return new Color32(255, 127, 80, 255);
				case 5: return new Color32(0, 255, 255, 255);
				case 6: return new Color32(255, 215, 0, 255);
				case 7: return new Color32(255, 0, 255, 255);
				case 8: return new Color32(50, 128, 120, 255);
				case 9: return new Color32(173, 255, 47, 255);
				case 10: return new Color32(255, 105, 180, 255);
				case 11: return new Color32(75, 0, 130, 255);
				case 12: return new Color32(255, 255, 240, 255);
				case 13: return new Color32(124, 252, 0, 255);
				case 14: return new Color32(255, 160, 122, 255);
				case 15: return new Color32(0, 255, 0, 255);
				case 16: return new Color32(245, 255, 250, 255);
				case 17: return new Color32(255, 228, 225, 255);
				case 18: return new Color32(218, 112, 214, 255);
				case 19: return new Color32(255, 192, 203, 255);
				case 20: return new Color32(255, 0, 0, 255);
				case 21: return new Color32(196, 112, 255, 255);
				case 22: return new Color32(250, 128, 114, 255);
				case 23: return new Color32(46, 139, 87, 255);
				case 24: return new Color32(192, 192, 192, 255);
				case 25: return new Color32(135, 206, 235, 255);
				case 26: return new Color32(0, 255, 127, 255);
				case 27: return new Color32(210, 180, 140, 255);
				case 28: return new Color32(0, 128, 128, 255);
				case 29: return new Color32(255, 99, 71, 255);
				case 30: return new Color32(64, 224, 208, 255);
				case 31: return new Color32(255, 255, 0, 255);
				case 32: return new Color32(154, 205, 50, 255);
			}
			return new Color32(240, 248, 255, 255); 
		}
        
		/// <summary>
		/// 将一个浮点数舍入到数组中最近的浮点数(数组必须被排序)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="array"></param>
		/// <returns></returns>
		public static float RoundFloatToArray(float value, float[] array)
		{
			int min = 0;
			if (array[min] >= value) return array[min];

			int max = array.Length - 1;
			if (array[max] <= value) return array[max];

			while (max - min > 1)
			{
				int mid = (max + min) / 2;

				if (array[mid] == value)
				{
					return array[mid];
				}
				else if (array[mid] < value)
				{
					min = mid;
				}
				else
				{
					max = mid;
				}
			}

			if (array[max] - value <= value - array[min])
			{
				return array[max];
			}
			else
			{
				return array[min];
			}
		}
}
