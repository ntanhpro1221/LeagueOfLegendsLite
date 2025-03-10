using System.Linq;
using UnityEngine;

namespace NGDtuanh.Utils {
    public static class RectExtensions {
        #region CHANGE TRANSFORM

        public static Rect With_Padding(this Rect rect, Padding padding) {
            var result = rect;
            result.xMin += padding.left;
            result.xMax -= padding.right;
            result.yMin += padding.top;
            result.yMax -= padding.bot;
            return result;
        }

        public static Rect With_X(this Rect rect, float x) {
            var result = rect;
            result.x = x;
            return result;
        }

        public static Rect With_Y(this Rect rect, float y) {
            var result = rect;
            result.y = y;
            return result;
        }

        public static Rect With_Width(this Rect rect, float width) {
            var result = rect;
            result.width = width;
            return result;
        }

        public static Rect With_Height(this Rect rect, float height) {
            var result = rect;
            result.height = height;
            return result;
        }

        public static float Get_Size(this Rect rect, Axis axis)
            => axis == Axis.Horizontal ? rect.width : rect.height;
        
        public static float Get_Pos(this Rect rect, Axis axis)
            => axis == Axis.Horizontal ? rect.x : rect.y;

        public static float Get_Center(this Rect rect, Axis axis)
            => axis == Axis.Horizontal ? rect.center.x : rect.center.y;
        
        public static float Assign_Size(this ref Rect rect, Axis axis, float value) {
            if (axis == Axis.Horizontal)
                return rect.width = value;
            return rect.height = value;
        }

        public static float Assign_Pos(this ref Rect rect, Axis axis, float value) {
            if (axis == Axis.Horizontal)
                return rect.x = value;
            return rect.y = value;
        }
        
        public static float Assign_Center(this ref Rect rect, Axis axis, float value) {
            if (axis == Axis.Horizontal)
                return (rect.center = new Vector2(value, rect.center.y)).x;
            return (rect.center = new Vector2(rect.center.x, value)).y;
        }
        
        public static Rect With_Size(this Rect rect, Axis axis, float size) {
            var result = rect;
            if (axis == Axis.Horizontal)
                result.width   = size;
            else result.height = size;
            return result;
        }

        public static Rect With_Size(this Rect rect, float width, float height) {
            var result = rect;
            result.width  = width;
            result.height = height;
            return result;
        }

        public static Rect With_Size(this Rect rect, Vector2 size) {
            var result = rect;
            result.size = size;
            return result;
        }
        
        public static Rect With_SizeKeepCenter(this Rect rect, Axis axis, float size) {
            return rect.With_Size(axis, size).With_CenterOf(axis,rect);
        }

        public static Rect With_Pos(this Rect rect, float x, float y) {
            var result = rect;
            result.x = x;
            result.y = y;
            return result;
        }

        public static Rect With_Pos(this Rect rect, Vector2 pos) {
            var result = rect;
            result.position = pos;
            return result;
        }

        public static Rect Move(this Rect rect, Vector2 delta) {
            var result = rect;
            result.position += delta;
            return result;
        }

        public static Rect Move(this Rect rect, float x, float y) {
            var result = rect;
            result.position += new Vector2(x, y);
            return result;
        }

        public static Rect Move(this Rect rect, Direction direction, float amount) {
            var result = rect;
            switch (direction) {
                case Direction.Left:
                    result.xMin -= amount;
                    result.xMax -= amount;
                    break;
                case Direction.Right:
                    result.xMin += amount;
                    result.xMax += amount;
                    break;
                case Direction.Top:
                    result.yMin -= amount;
                    result.yMax -= amount;
                    break;
                case Direction.Bottom:
                    result.yMin += amount;
                    result.yMax += amount;
                    break;
            }
            
            return result;
        }
        
        public static Rect With_CenterOf(this Rect rect, Axis axis, Rect pattern) {
            var result = rect;

            result.Assign_Center(axis, pattern.Get_Center(axis));
            
            return result;
        }

        public static Rect Move(this Rect rect, Axis axis, float amount) {
            var result = rect;
            switch (axis) {
                case Axis.Horizontal:
                    result.xMin += amount;
                    result.xMax += amount;
                    break;
                case Axis.Vertical:
                    result.yMin += amount;
                    result.yMax += amount;
                    break;
            }

            return result;
        }

        #endregion

        public static Rect CutBySize(this Rect rect, Direction direction, float amount) {
            var result = rect;

            switch (direction) {
                case Direction.Left:   result.xMin += Mathf.Clamp(amount, 0, rect.width); break;
                case Direction.Right:  result.xMax -= Mathf.Clamp(amount, 0, rect.width); break;
                case Direction.Top:    result.yMin += Mathf.Clamp(amount, 0, rect.height); break;
                case Direction.Bottom: result.yMax -= Mathf.Clamp(amount, 0, rect.height); break;
            }

            return result;
        }

        public static Rect CutByRatio(this Rect rect, Direction direction, float ratio) {
            var result = rect;

            switch (direction) {
                case Direction.Left:   result.xMin += Mathf.Clamp(rect.width  * ratio, 0, rect.width); break;
                case Direction.Right:  result.xMax -= Mathf.Clamp(rect.width  * ratio, 0, rect.width); break;
                case Direction.Top:    result.yMin += Mathf.Clamp(rect.height * ratio, 0, rect.height); break;
                case Direction.Bottom: result.yMax -= Mathf.Clamp(rect.height * ratio, 0, rect.height); break;
            }

            return result;
        }

        public static (Rect, Rect) DivideBySize(this Rect rect, Direction direction, float size) {
            var result = (rect, rect);

            switch (direction) {
                case Direction.Left:   result.Item1.xMax = result.Item2.xMin = rect.xMin + Mathf.Clamp(size, 0, rect.width); break;
                case Direction.Right:  result.Item1.xMax = result.Item2.xMin = rect.xMax - Mathf.Clamp(size, 0, rect.width); break;
                case Direction.Top:    result.Item1.yMax = result.Item2.yMin = rect.yMin + Mathf.Clamp(size, 0, rect.height); break;
                case Direction.Bottom: result.Item1.yMax = result.Item2.yMin = rect.yMax - Mathf.Clamp(size, 0, rect.height); break;
            }

            return result;
        }

        public static (Rect, Rect) DivideByRatio(this Rect rect, Direction direction, float ratio) {
            var result = (rect, rect);

            switch (direction) {
                case Direction.Left:   result.Item1.xMax = result.Item2.xMin = rect.xMin + Mathf.Clamp(rect.width  * ratio, 0, rect.width); break;
                case Direction.Right:  result.Item1.xMax = result.Item2.xMin = rect.xMax - Mathf.Clamp(rect.width  * ratio, 0, rect.width); break;
                case Direction.Top:    result.Item1.yMax = result.Item2.yMin = rect.yMin + Mathf.Clamp(rect.height * ratio, 0, rect.height); break;
                case Direction.Bottom: result.Item1.yMax = result.Item2.yMin = rect.yMax - Mathf.Clamp(rect.height * ratio, 0, rect.height); break;
            }

            return result;
        }

        public static Rect[] DivideByAllSizes(this Rect rect, Axis axis, int alignLeftTopAmount, float space, params float[] sizes) {
            var result = new Rect[sizes.Length];
            if (sizes.Length == 0) return result;

            alignLeftTopAmount = Mathf.Clamp(alignLeftTopAmount, 0, sizes.Length);

            // HANDLE NOT ENOUGH SPACE
            float sumSize       = sizes.Sum();
            float availableSize = rect.Get_Size(axis) - space * (sizes.Length - 1);
            if (sumSize > availableSize) {
                float scaleSolver     = availableSize / sumSize;
                float tmpAvailableSize = availableSize;
                for (int i = 1 // yes it is 1
                     ; i < sizes.Length
                     ; ++i)
                    tmpAvailableSize -= sizes[i] *= scaleSolver;
                sizes[0] = tmpAvailableSize;
                sumSize  = availableSize;
            }
            float leftRightSpace = availableSize - sumSize;
            
            // APPLY TO RESULT
            Rect curPattern = rect;
            for (int i = 0; i < sizes.Length; ++i) {
                curPattern = curPattern.Move(axis, leftRightSpace.IfOnly(i == alignLeftTopAmount));
                result[i]  = curPattern.With_Size(axis, sizes[i]);
                curPattern = result[i].Move(axis, sizes[i] + space);
            }

            return result;
        }
    }
}