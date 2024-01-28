using UnityEngine;

namespace F8Framework.Core
{
    public enum PivotPreset
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight
    }

    /// <summary>
    /// This script help you convert anchored positions to absolute positions, so you can animate and position exactly ui elements witouh messing around with anchors
    /// </summary>
    

    //use this to position UI in absolute coordenates
    //0.0 , 1.0 _______________________1.0 , 1.0
    //          |                      |
    //          |                      |                  
    //          |                      |
    //          |                      |
    //0.0 , 0.0 |______________________| 1.0 , 0.0

    public static class RectTransformHelper
    {
        private static RectTransform targetCanvas = null;

        public static Vector2 FromAbsolutePositionToAnchoredPosition(this RectTransform rect, Vector2 point, RectTransform canvas, PivotPreset anchor = PivotPreset.MiddleCenter)
        {            
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;          
            return Vector2.Scale( point - centerAnchor, canvas.rect.size ) + Vector2.Scale( rect.rect.size, GetAnchorOffSet( anchor ) );
        }

        public static Vector2 FromAbsolutePositionToAnchoredPosition(this RectTransform rect, Vector2 point, PivotPreset anchor = PivotPreset.MiddleCenter)
        {
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;
            return Vector2.Scale( point - centerAnchor, RequestCanvas().rect.size ) + Vector2.Scale( rect.rect.size, GetAnchorOffSet( anchor ) );
        }

        public static Vector2 FromAbsolutePositionToAnchoredPosition(this RectTransform rect, Vector2 point, RectTransform canvas, Vector2 pivot)
        {
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;
            return Vector2.Scale( point - centerAnchor, canvas.rect.size ) + Vector2.Scale( rect.rect.size, pivot * 0.5f );
        }
      
        public static Vector2 FromAnchoredPositionToAbsolutePosition(this RectTransform rect, RectTransform canvas , PivotPreset anchor = PivotPreset.MiddleCenter)
        {
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition -= Vector2.Scale( rect.rect.size, GetAnchorOffSet( anchor ) );
            return new Vector2( anchoredPosition.x/ canvas.sizeDelta.x, anchoredPosition.y / canvas.sizeDelta.y ) + centerAnchor ;
        }        

        public static Vector2 FromAnchoredPositionToAbsolutePosition(this RectTransform rect , PivotPreset anchor = PivotPreset.MiddleCenter)
        {
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition -= Vector2.Scale( rect.rect.size, GetAnchorOffSet( anchor ) );
            return new Vector2( anchoredPosition.x / RequestCanvas().sizeDelta.x, anchoredPosition.y / RequestCanvas().sizeDelta.y ) + centerAnchor;
        }

        public static Vector2 FromAnchoredPositionToAbsolutePosition(this RectTransform rect, RectTransform canvas, Vector2 pivot)
        {
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition -= Vector2.Scale( rect.rect.size, pivot * 0.5f );
            return new Vector2( anchoredPosition.x / canvas.sizeDelta.x, anchoredPosition.y / canvas.sizeDelta.y ) + centerAnchor;
        }

        public static Vector2 FromAnchoredPositionToAbsolutePosition(this RectTransform rect, Vector2 pivot)
        {
            Vector2 centerAnchor = ( rect.anchorMax + rect.anchorMin ) * 0.5f;
            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition -= Vector2.Scale( rect.rect.size, pivot * 0.5f );
            return new Vector2( anchoredPosition.x / RequestCanvas().sizeDelta.x, anchoredPosition.y / RequestCanvas().sizeDelta.y ) + centerAnchor;
        }

        public static Vector2 FromRectSizeToAbsoluteSize(this RectTransform rect , RectTransform canvas)
        {
            return new Vector2( rect.rect.size.x / canvas.sizeDelta.x, rect.rect.size.y / canvas.sizeDelta.y );
        }

        public static Vector2 FromRectSizeToAbsoluteSize(this RectTransform rect)
        {
            return new Vector2( rect.rect.size.x / RequestCanvas().sizeDelta.x, rect.rect.size.y / RequestCanvas().sizeDelta.y );
        }

        public static void UpdatePivot(this RectTransform rect , PivotPreset pivot)
        {
            UpdatePivot(rect , GetPivot(pivot));
        }

        public static void UpdatePivot(this RectTransform rect, Vector2 pivot)
        {
            RectTransform canvas = RequestCanvas();

            Vector2 size = rect.rect.size;
            Vector2 deltaPivot = rect.pivot - pivot;

            //Vector2 deltaPosition = new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);
            Vector2 deltaPosition = new Vector2( rect.right.x , rect.right.y ).normalized * rect.rect.size.x * deltaPivot.x;
            deltaPosition += new Vector2(rect.up.x, rect.up.y).normalized * rect.rect.size.y * deltaPivot.y;

            rect.pivot = pivot;
            rect.anchoredPosition -= deltaPosition;

        }

        private static RectTransform RequestCanvas()
        {
            //we have a canvas in cache?
            if (targetCanvas != null)
                return targetCanvas;

            Canvas canvas = null;

            //just return the first encounter canvas
            canvas = GameObject.FindObjectOfType<Canvas>();

            if (canvas != null)
            {
                targetCanvas = canvas.GetComponent<RectTransform>();
                return targetCanvas;
            }
                

            return null;
        }

        private static Vector2 GetAnchorOffSet(PivotPreset anchor)
        {
            switch (anchor)
            {
                case PivotPreset.UpperLeft:
                return new Vector2( 0.5f, -0.5f );
                case PivotPreset.UpperCenter:
                return new Vector2( 0.0f, -0.5f );
                case PivotPreset.UpperRight:
                return new Vector2( -0.5f, -0.5f );
                case PivotPreset.MiddleLeft:
                return new Vector2( 0.5f, 0.0f );
                case PivotPreset.MiddleCenter:
                return new Vector2( 0.0f, 0.0f );
                case PivotPreset.MiddleRight:
                return new Vector2( -0.5f, 0.0f );
                case PivotPreset.LowerLeft:
                return new Vector2( 0.5f, 0.5f );
                case PivotPreset.LowerCenter:
                return new Vector2( 0.0f, 0.5f );
                case PivotPreset.LowerRight:
                return new Vector2( -0.5f, 0.5f );
            }

            return Vector2.zero;

        }

        private static Vector2 GetPivot(PivotPreset pivot)
        {
            switch (pivot)
            {
                case PivotPreset.UpperLeft:
                    return new Vector2(0.0f, 1.0f);
                case PivotPreset.UpperCenter:
                    return new Vector2(0.5f, 1.0f);
                case PivotPreset.UpperRight:
                    return new Vector2(1.0f, 1.0f);
                case PivotPreset.MiddleLeft:
                    return new Vector2(0.0f, 0.5f);
                case PivotPreset.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);
                case PivotPreset.MiddleRight:
                    return new Vector2(1.0f, 0.5f);
                case PivotPreset.LowerLeft:
                    return new Vector2(0.0f, 0.0f);
                case PivotPreset.LowerCenter:
                    return new Vector2(0.5f, 0.0f);
                case PivotPreset.LowerRight:
                    return new Vector2(1.0f, 0.0f);
            }

            return Vector2.zero;

        }
    }
}

