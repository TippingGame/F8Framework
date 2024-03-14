using System.Linq;
using UnityEngine;

namespace F8Framework.Core.Editor
{
	public static class Slicer
	{
		public static SlicedTexture Slice(Texture2D texture, SliceOptions options)
		{
			return (new Runner(texture, options).Run());
		}

		private class Runner
		{
			private readonly Texture2D _texture;
			private readonly SliceOptions _options;
			private int _width;
			private int _height;
			private Color32[] _pixels;

			public Runner(Texture2D texture, SliceOptions options)
			{
				_texture = texture;
				_options = options;
			}

			public SlicedTexture Run()
			{
				_width = _texture.width;
				_height = _texture.height;
				_pixels = _texture.GetPixels().Select(x => (Color32) x).ToArray();
				for (var i = 0; i < _pixels.Length; ++i) _pixels[i] = _pixels[i].a > 0 ? _pixels[i] : (Color32) Color.clear;

				var xDiffList = CalcDiffList(_width, _height, 1, _width);
				var (xStart, xEnd) = CalcLine(xDiffList);

				var yDiffList = CalcDiffList(1, _width, _width, _height);
				var (yStart, yEnd) = CalcLine(yDiffList);

				var skipX = (xStart == 0 && xEnd == 0);
				var skipY = (yStart == 0 && yEnd == 0);
				var output = GenerateSlicedTexture(xStart, xEnd, yStart, yEnd, skipX, skipY);

				var left = xStart;
				var bottom = yStart;
				var right = (_width - xEnd) - 1;
				var top = (_height - yEnd) - 1;

				if (skipX)
				{
					left = 0;
					right = 0;
				}

				if (skipY)
				{
					top = 0;
					bottom = 0;
				}

				return new SlicedTexture(output, new Border(left, bottom, right, top));
			}

			private ulong[] CalcDiffList(int lineDelta, int lineLength, int lineSeek, int length)
			{
				var diffList = new ulong[length];
				diffList[0] = ulong.MaxValue;

				for (var i = 1; i < length; ++i)
				{
					ulong diff = 0;
					var current = i * lineSeek;
					for (var j = 0; j < lineLength; ++j)
					{
						var prev = current - lineSeek;
						diff += (ulong) Diff(_pixels[prev], _pixels[current]);
						current += lineDelta;
					}
					diffList[i] = diff;
				}

				return diffList;
			}

			private int Diff(Color32 a, Color32 b)
			{
				var rd = Mathf.Abs(a.r - b.r);
				var gd = Mathf.Abs(a.g - b.g);
				var bd = Mathf.Abs(a.b - b.b);
				var ad = Mathf.Abs(a.a - b.a);
				if (rd <= _options.Tolerate) rd = 0;
				if (gd <= _options.Tolerate) gd = 0;
				if (bd <= _options.Tolerate) bd = 0;
				if (ad <= _options.Tolerate) ad = 0;
				return rd + gd + bd + ad;
			}

			private (int Start, int End) CalcLine(ulong[] list)
			{
				var start = 0;
				var end = 0;
				var tmpStart = 0;
				var tmpEnd = 0;
				for (var i = 0; i < list.Length; ++i)
				{
					if (list[i] == 0)
					{
						tmpEnd = i;
						continue;
					}

					if (end - start < tmpEnd - tmpStart)
					{
						start = tmpStart;
						end = tmpEnd;
					}

					tmpStart = i;
					tmpEnd = i;
				}

				if (end - start < tmpEnd - tmpStart)
				{
					start = tmpStart;
					end = tmpEnd;
				}

				start += _options.Margin;
				end -= _options.Margin;

				if (end <= start)
				{
					start = 0;
					end = 0;
				}

				return (start, end);
			}

			private Texture2D GenerateSlicedTexture(int xStart, int xEnd, int yStart, int yEnd, bool skipX, bool skipY)
			{
				var outputWidth = _width - (xEnd - xStart) + (skipX ? 0 : _options.CenterSize - 1);
				var outputHeight = _height - (yEnd - yStart) + (skipY ? 0 : _options.CenterSize - 1);
				var outputPixels = new Color[outputWidth * outputHeight];
				for (int x = 0, originalX = 0; x < outputWidth; ++x, ++originalX)
				{
					if (originalX == xStart && !skipX) originalX += (xEnd - xStart) - _options.CenterSize + 1;
					for (int y = 0, originalY = 0; y < outputHeight; ++y, ++originalY)
					{
						if (originalY == yStart && !skipY) originalY += (yEnd - yStart) - _options.CenterSize + 1;
						outputPixels[y * outputWidth + x] = Get(originalX, originalY);
					}
				}

				var output = new Texture2D(outputWidth, outputHeight);
				output.SetPixels(outputPixels);
				return output;
			}

			private Color32 Get(int x, int y)
			{
				return _pixels[y * _width + x];
			}
		}
	}
}
