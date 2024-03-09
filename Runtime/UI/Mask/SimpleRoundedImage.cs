using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public class SimpleRoundedImage : Image
    {
        //每个角最大的三角形数，一般5-8个就有不错的圆角效果，设置Max防止不必要的性能浪费
        const int MaxTriangleNum = 20;
        const int MinTriangleNum = 1;

        public float Radius;

        //使用几个三角形去填充每个角的四分之一圆
        [Range(MinTriangleNum, MaxTriangleNum)]
        public int TriangleNum = 1;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Vector4 v = GetDrawingDimensions(false);
            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            var color32 = color;
            vh.Clear();
            //对radius的值做限制，必须在0-较小的边的1/2的范围内
            float radius = Radius;
            if (radius > (v.z - v.x) / 2) radius = (v.z - v.x) / 2;
            if (radius > (v.w - v.y) / 2) radius = (v.w - v.y) / 2;
            if (radius < 0) radius = 0;
            //计算出uv中对应的半径值坐标轴的半径
            float uvRadiusX = radius / (v.z - v.x);
            float uvRadiusY = radius / (v.w - v.y);

            //0，1
            vh.AddVert(new Vector3(v.x, v.w - radius), color32, new Vector2(uv.x, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.x, v.y + radius), color32, new Vector2(uv.x, uv.y + uvRadiusY));

            //2，3，4，5
            vh.AddVert(new Vector3(v.x + radius, v.w), color32, new Vector2(uv.x + uvRadiusX, uv.w));
            vh.AddVert(new Vector3(v.x + radius, v.w - radius), color32,
                new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.x + radius, v.y + radius), color32,
                new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
            vh.AddVert(new Vector3(v.x + radius, v.y), color32, new Vector2(uv.x + uvRadiusX, uv.y));

            //6，7，8，9
            vh.AddVert(new Vector3(v.z - radius, v.w), color32, new Vector2(uv.z - uvRadiusX, uv.w));
            vh.AddVert(new Vector3(v.z - radius, v.w - radius), color32,
                new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.z - radius, v.y + radius), color32,
                new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
            vh.AddVert(new Vector3(v.z - radius, v.y), color32, new Vector2(uv.z - uvRadiusX, uv.y));

            //10，11
            vh.AddVert(new Vector3(v.z, v.w - radius), color32, new Vector2(uv.z, uv.w - uvRadiusY));
            vh.AddVert(new Vector3(v.z, v.y + radius), color32, new Vector2(uv.z, uv.y + uvRadiusY));

            //左边的矩形
            vh.AddTriangle(1, 0, 3);
            vh.AddTriangle(1, 3, 4);
            //中间的矩形
            vh.AddTriangle(5, 2, 6);
            vh.AddTriangle(5, 6, 9);
            //右边的矩形
            vh.AddTriangle(8, 7, 10);
            vh.AddTriangle(8, 10, 11);

            //开始构造四个角
            List<Vector2> vCenterList = new List<Vector2>();
            List<Vector2> uvCenterList = new List<Vector2>();
            List<int> vCenterVertList = new List<int>();

            //右上角的圆心
            vCenterList.Add(new Vector2(v.z - radius, v.w - radius));
            uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.w - uvRadiusY));
            vCenterVertList.Add(7);

            //左上角的圆心
            vCenterList.Add(new Vector2(v.x + radius, v.w - radius));
            uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.w - uvRadiusY));
            vCenterVertList.Add(3);

            //左下角的圆心
            vCenterList.Add(new Vector2(v.x + radius, v.y + radius));
            uvCenterList.Add(new Vector2(uv.x + uvRadiusX, uv.y + uvRadiusY));
            vCenterVertList.Add(4);

            //右下角的圆心
            vCenterList.Add(new Vector2(v.z - radius, v.y + radius));
            uvCenterList.Add(new Vector2(uv.z - uvRadiusX, uv.y + uvRadiusY));
            vCenterVertList.Add(8);

            //每个三角形的顶角
            float degreeDelta = (float)(Mathf.PI / 2 / TriangleNum);
            //当前的角度
            float curDegree = 0;

            for (int i = 0; i < vCenterVertList.Count; i++)
            {
                int preVertNum = vh.currentVertCount;
                for (int j = 0; j <= TriangleNum; j++)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    Vector3 vPosition = new Vector3(vCenterList[i].x + cosA * radius, vCenterList[i].y + sinA * radius);
                    Vector3 uvPosition = new Vector2(uvCenterList[i].x + cosA * uvRadiusX,
                        uvCenterList[i].y + sinA * uvRadiusY);
                    vh.AddVert(vPosition, color32, uvPosition);
                    curDegree += degreeDelta;
                }

                curDegree -= degreeDelta;
                for (int j = 0; j <= TriangleNum - 1; j++)
                {
                    vh.AddTriangle(vCenterVertList[i], preVertNum + j + 1, preVertNum + j);
                }
            }
        }

        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
            Rect r = GetPixelAdjustedRect();
            var size = overrideSprite == null
                ? new Vector2(r.width, r.height)
                : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
            //Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            var v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

            v = new Vector4(
                r.x + r.width * v.x,
                r.y + r.height * v.y,
                r.x + r.width * v.z,
                r.y + r.height * v.w
            );

            return v;
        }
    }
}