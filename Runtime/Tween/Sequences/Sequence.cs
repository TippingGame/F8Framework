using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
	public enum PlayMode
	{
		Append,
		Join
	}

	public class Sequence : IRecyclable<Sequence>
	{
		private List<BaseTween> tweenList = new List<BaseTween>();
		private List<TimeEvent> timeEvents = new List<TimeEvent>();
		
		private float timer = 0.0f;
		private BaseTween head = null;
		private List<Command> commandQueue = new List<Command>();
		private int loops = 0;
		private bool ignoreCommands = false;

		public Action OnComplete = null;
		
		public Action<Sequence> Recycle { get; set; }

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
					timeEvents.RemoveAt(n);
					n--;
				}
			}

		}

		public void SetOnComplete(Action action)
		{
			OnComplete += action;
		}

		private void CheckLoops()
		{
			tweenList.Clear();
			
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
					tween.Recycle(tween);
				}
				
				
				//we are ready to let this object go
				Recycle?.Invoke(this);
			}
		}

		private void RunAigan()
		{
			timer = 0.0f;
			OnComplete = null;
			
			OnComplete += CheckLoops;
			ignoreCommands = true;
			
			for (int n = 0; n < commandQueue.Count; n++)
			{
				commandQueue[n].Execute();
			}

		}

		public void Append(BaseTween tween)
		{
			tween.HandleBySequence = true;
			
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
			tween.HandleBySequence = true;
			
			if (ShouldPlayInmediatly())
			{
				PlayTweenInmediatly(tween, PlayMode.Join);
				return;
			}

			var tweenP = GetPenultimate();
			PlayTweenOnComplete(tween, tweenP, PlayMode.Join);
		}

		private void PlayTweenInmediatly(BaseTween tween, PlayMode playMode)
		{
			tweenList.Add(tween);
			
			tween.SetOnComplete(CheckIfSequenceIsComplete);
			tween.SetOnComplete(() => tween.SetIsPause(true));

			if (!ignoreCommands)
			{
				
				commandQueue.Add(GetCommand(playMode, tween));
			}
		}

		private void PlayTweenOnComplete(BaseTween tween, BaseTween previousTween, PlayMode playMode)
		{
			tweenList.Add(tween);
			
			//pause the tween until it can actually run
			tween.SetIsPause(true);
			tween.SetOnComplete(() => tween.SetIsPause(true));
			previousTween.SetOnComplete(delegate { RunTween(tween); });
			
			tween.SetOnComplete(CheckIfSequenceIsComplete);

			if (!ignoreCommands)
			{
				
				commandQueue.Add(GetCommand(playMode, tween));
			}
		}

		private Command GetCommand(PlayMode mode, BaseTween tween)
		{
			if (mode == PlayMode.Append)
			{
				return  new AppendTweenCommand(tween, this);
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
			
			head.SetOnComplete(callback);
		}

		public void Join(Action callback)
		{
			if (ShouldPlayInmediatly())
			{
				callback.Invoke();
				return;
			}
			
			GetPenultimate().SetOnComplete(callback);
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
			tweenList.Clear();
			timeEvents.Clear();
			timer = 0.0f;
			OnComplete = null;
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
			tween.SetOnComplete(() => tween.IsComplete = true);
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
			tween.SetOnComplete(() => tween.IsComplete = true);
			sequence.Join(tween);
		}
	}
	
}

