using UnityEngine;
using UnityEngine.Playables;

namespace F8Framework.Core
{
	[System.Serializable]
	public class TextLocalizerPlayableAsset : PlayableAsset
	{
		public string textId;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			var playable = ScriptPlayable<TextLocalizerPlayableBehaviour>.Create(graph);

			var behaviour = playable.GetBehaviour();
			behaviour.textId = textId;

			return playable;
		}
	}
}
