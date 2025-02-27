using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace F8Framework.Core
{
	public enum PlayMode
	{
		Append,
		Join
	}

	public class Sequence : IEnumerator
	{
		private List<BaseTween> tweenList = new List<BaseTween>();
		private List<TimeEvent> timeEvents = new List<TimeEvent>();
		
		private float timer = 0.0f;
		private BaseTween head = null;
		private List<Command> commandQueue = new List<Command>();
		private int loops = 0;
		private bool ignoreCommands = false;

		public Action OnComplete = null;
		
		public Action Recycle { get; set; }

		public Sequence()
		{
			OnComplete += CheckLoops;
		}

		public void Update(float deltaTime)
		{
			timer += deltaTime;

			for (int n = 0; n < timeEvents.Count ; n++)
			{
				if (timeEvents[n].Time < timer)
				{
					timeEvents[n].Action.Invoke();
					if (timeEvents.Count > 0)
					{
						timeEvents.RemoveAt(n);
						n--;
					}
				}
			}

		}

		public void SetOnComplete(Action action)
		{
			Recycle += action;
		}

		private void CheckLoops()
		{
			if (loops < 0 || loops > 0)
			{
				loops--;
				RunAigan();
			}
			else
			{
				//recicle all tweens
				foreach (var tween in tweenList)
				{
					tween.CanRecycle = true;
					tween.IsRecycle = true;
				}
				tweenList.Clear();
				
				//we are ready to let this object go
				Recycle?.Invoke();
			}
		}

		private void RunAigan()
		{
			timer = 0.0f;
			ignoreCommands = true;
			
			for (int n = 0; n < commandQueue.Count; n++)
			{
				commandQueue[n].Execute();
			}

		}

		public void Append(BaseTween tween)
		{
			tween.CanRecycle = false;
			
			if (ShouldPlayInmediatly())
			{
				PlayTweenInmediatly(tween, PlayMode.Append);
				head = tween;
				return;
			}
			
			PlayTweenOnComplete(tween, head, PlayMode.Append);
			head = tween;
		}
		
		public void Join(BaseTween tween)
		{
			tween.CanRecycle = false;
			
			if (ShouldPlayInmediatly())
			{
				PlayTweenInmediatly(tween, PlayMode.Join);
				head = tween;
				return;
			}
			
			PlayTweenOnComplete(tween, head, PlayMode.Join);
			head = tween;
		}

		private void PlayTweenInmediatly(BaseTween tween, PlayMode playMode)
		{
			tweenList.Add(tween);
			
			tween.SetOnCompleteSequence(CheckIfSequenceIsComplete);
			tween.SetOnCompleteSequence(() => tween.SetIsPause(true));

			if (!ignoreCommands)
			{
				commandQueue.Add(GetCommand(playMode, tween));
			}
		}

		private void PlayTweenOnComplete(BaseTween tween, BaseTween previousTween, PlayMode playMode)
		{
			tweenList.Add(tween);

			// 如果是 Append 模式，让动画暂停直到上一个动画完成
			if (playMode == PlayMode.Append)
			{
				tween.SetIsPause(true);
			}
    
			previousTween.SetOnCompleteSequence(() => RunTween(tween));
			tween.SetOnCompleteSequence(CheckIfSequenceIsComplete);

			if (!ignoreCommands)
			{
				commandQueue.Add(GetCommand(playMode, tween));
			}
		}

		private Command GetCommand(PlayMode mode, BaseTween tween)
		{
			if (mode == PlayMode.Append)
			{
				return new AppendTweenCommand(tween, this);
			}
			else
			{
				return new JoinTweenCommand(tween, this);
			}
		}

		private bool ShouldPlayInmediatly()
		{
			return head == null || head.IsComplete;
		}

		public void SetLoops(int loops)
		{
			if(loops == 0 || loops == 1)
				return;
			
			this.loops = loops - 1;
		}

		public void Append(Action callback)
		{
			if (ShouldPlayInmediatly())
			{
				callback.Invoke();
				return;
			}
			
			head.SetOnCompleteSequence(callback);
		}

		public void Join(Action callback)
		{
			if (ShouldPlayInmediatly())
			{
				callback.Invoke();
				return;
			}
			
			GetPenultimate().SetOnCompleteSequence(callback);
		}

		public void RunAtTime(Action callback, float time)
		{
			timeEvents.Add(new TimeEvent(callback, time));
		}
		
		public void RunAtTime(BaseTween tween, float time)
		{
			tween.SetIsPause(true);
			timeEvents.Add(new TimeEvent(delegate { RunTween(tween); }, time));
		}

		public void Reset()
		{
			foreach (var tween in tweenList)
			{
				tween.CanRecycle = true;
				tween.IsRecycle = true;
			}
			tweenList.Clear();
			timeEvents.Clear();
			timer = 0.0f;
			loops = 0;
			ignoreCommands = false;
			commandQueue.Clear();
			head = null;
			Recycle = null;
		}

		private void RunTween(BaseTween tween)
		{
			tween.SetIsPause(false);
		}

		private void CheckIfSequenceIsComplete()
		{
			foreach (var tween in tweenList)
			{
				if (!tween.IsComplete)
				{
					return;
				}
			}

			if(OnComplete != null)
				OnComplete.Invoke();
		}

		private BaseTween GetPenultimate()
		{
			if (tweenList.Count <= 0)
				return null;
			if (tweenList.Count < 2)
				return tweenList[0];

			return tweenList[tweenList.Count - 2];
		}

		/// <summary>使用此方法在协程中等待Sequence。</summary>
		/// <example><code>
		/// IEnumerator Coroutine() {
		///		var sequence = SequenceManager.GetSequence();
		///		var baseTween = gameObject.Move(Vector3.one, 1f);
		///		sequence.Append(baseTween);
		///		yield return sequence;
		/// }
		/// </code></example>
		bool IEnumerator.MoveNext() {
			return Recycle != null;
		}

		object IEnumerator.Current {
			get {
				if (Recycle == null)
				{
					LogF8.LogError("已回收的Sequence无法访问当前值");
				}
				return null;
			}
		}

		void IEnumerator.Reset() => throw new NotSupportedException();
        
		/// <summary>此方法是异步/等待支持所必需的。不要直接使用它。</summary>
		/// <example><code>
		/// async void Coroutine() {
		///		var sequence = SequenceManager.GetSequence();
		///		var baseTween = gameObject.Move(Vector3.one, 1f);
		///		sequence.Append(baseTween);
		///     await sequence;
		/// }
		/// </code></example>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SwquenceAwaiter GetAwaiter() {
			return new SwquenceAwaiter(this);
		}
	}

	public abstract class Command
	{
		protected Sequence sequence;

		protected Command(Sequence sequence)
		{
			this.sequence = sequence;
		}

		public abstract void Execute();
	}

	public class AppendTweenCommand : Command
	{
		public BaseTween tween;

		public AppendTweenCommand(BaseTween tween, Sequence sequence) : base(sequence)
		{
			this.tween = tween;
		}


		public override void Execute()
		{
			tween.ReplayReset();
			sequence.Append(tween);
		}
	}
	
	public class JoinTweenCommand : Command
	{
		public BaseTween tween;

		public JoinTweenCommand(BaseTween tween, Sequence sequence) : base(sequence)
		{
			this.tween = tween;
		}

		public override void Execute()
		{
			tween.ReplayReset();
			sequence.Join(tween);
		}
	}
	
}

