using UnityEngine.Playables;

namespace F8Framework.Core
{
	public class TextLocalizerPlayableBehaviour : PlayableBehaviour
	{
		public string textId;
		TextLocalizer textLocalizer;

		public override void OnBehaviourPlay(Playable playable, FrameData info) { }

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (textLocalizer != null) textLocalizer.Clear();
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			textLocalizer = playerData as TextLocalizer;
			if (textLocalizer != null) textLocalizer.ChangeID(textId);
		}
	}
}
