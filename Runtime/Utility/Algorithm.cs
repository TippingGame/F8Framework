using System;
using System.Text;
using System.Collections.Generic;

namespace F8Framework.Core
{
    public static partial class Util
    {
        /// <summary>
        /// 通用算法工具类，封装了常用算法。
        /// </summary>
        public static class Algorithm
        {
            static Random random = new Random(Guid.NewGuid().GetHashCode());

            /// <summary>
            /// 快速排序：降序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            /// <param name="start">起始位</param>
            /// <param name="end">结束位</param>
            public static void SortByDescend<T, K>(IList<T> array, Func<T, K> handler, int start, int end)
                where K : IComparable<K>
            {
                if (array == null)
                    throw new ArgumentNullException("SortByDescend : array is null");
                if (handler == null)
                    throw new ArgumentNullException("SortByDescend : handler is null");
                if (start < 0 || end < 0 || start >= end)
                {
                    return;
                }

                int pivort = start;
                T pivortValue = array[pivort];
                Swap(array, end, pivort);
                int storeIndex = start;
                for (int i = start; i <= end - 1; i++)
                {
                    if (handler(array[i]).CompareTo(handler(pivortValue)) > 0)
                    {
                        Swap(array, i, storeIndex);
                        storeIndex++;
                    }
                }

                Swap(array, storeIndex, end);
                SortByDescend(array, handler, start, storeIndex - 1);
                SortByDescend(array, handler, storeIndex + 1, end);
            }

            /// <summary>
            /// 快速排序：升序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            /// <param name="start">起始位</param>
            /// <param name="end">结束位</param>
            public static void SortByAscend<T, K>(IList<T> array, Func<T, K> handler, int start, int end)
                where K : IComparable<K>
            {
                if (array == null)
                    throw new ArgumentNullException("QuickSortByAscend : array is null");
                if (handler == null)
                    throw new ArgumentNullException("QuickSortByAscend : handler is null");
                if (start < 0 || end < 0 || start >= end)
                {
                    return;
                }

                int pivort = start;
                T pivortValue = array[pivort];
                Swap(array, end, pivort);
                int storeIndex = start;
                for (int i = start; i <= end - 1; i++)
                {
                    if (handler(array[i]).CompareTo(handler(pivortValue)) < 0)
                    {
                        Swap(array, i, storeIndex);
                        storeIndex++;
                    }
                }

                Swap(array, storeIndex, end);
                SortByAscend(array, handler, start, storeIndex - 1);
                SortByAscend(array, handler, storeIndex + 1, end);
            }

            /// <summary>
            /// 冒泡排序：升序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            public static void SortByAscend<T, K>(IList<T> array, Func<T, K> handler)
                where K : IComparable<K>
            {
                for (int i = 0; i < array.Count; i++)
                {
                    for (int j = 0; j < array.Count; j++)
                    {
                        if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                        {
                            (array[i], array[j]) = (array[j], array[i]);
                        }
                    }
                }
            }

            /// <summary>
            /// 冒泡排序：降序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            public static void SortByDescend<T, K>(IList<T> array, Func<T, K> handler)
                where K : IComparable<K>
            {
                for (int i = 0; i < array.Count; i++)
                {
                    for (int j = 0; j < array.Count; j++)
                    {
                        if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                        {
                            (array[i], array[j]) = (array[j], array[i]);
                        }
                    }
                }
            }

            /// <summary>
            /// 冒泡排序：升序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="comparison">排序条件</param>
            public static void SortByAscend<T, K>(IList<T> array, Comparison<T> comparison)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    for (int j = 0; j < array.Count; j++)
                    {
                        if (comparison(array[i], array[j]) < 0)
                        {
                            (array[i], array[j]) = (array[j], array[i]);
                        }
                    }
                }
            }

            /// <summary>
            /// 冒泡排序：降序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="comparison">排序条件</param>
            public static void SortByDescend<T, K>(IList<T> array, Comparison<T> comparison)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    for (int j = 0; j < array.Count; j++)
                    {
                        if (comparison(array[i], array[j]) > 0)
                        {
                            (array[i], array[j]) = (array[j], array[i]);
                        }
                    }
                }
            }

            /// <summary>
            ///  获取最小
            /// </summary>
            public static T Min<T, K>(IList<T> array, Func<T, K> handler)
                where K : IComparable<K>
            {
                T temp = default(T);
                temp = array[0];
                foreach (var arr in array)
                {
                    if (handler(temp).CompareTo(handler(arr)) > 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获取最大值
            /// </summary>
            public static T Max<T, K>(IList<T> array, Func<T, K> handler)
                where K : IComparable<K>
            {
                T temp = default(T);
                temp = array[0];
                foreach (var arr in array)
                {
                    if (handler(temp).CompareTo(handler(arr)) < 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            ///  获取最小
            /// </summary>
            public static T Min<T, K>(IList<T> array, Comparison<T> comparison)
            {
                T temp = default(T);
                temp = array[0];
                foreach (var arr in array)
                {
                    if (comparison(temp, arr) > 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获取最大值
            /// </summary>
            public static T Max<T, K>(IList<T> array, Comparison<T> comparison)
            {
                T temp = default(T);
                temp = array[0];
                foreach (var arr in array)
                {
                    if (comparison(temp, arr) < 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获得传入元素某个符合条件的所有对象
            /// </summary>
            public static T Find<T>(IList<T> array, Predicate<T> handler)
            {
                T temp = default(T);
                for (int i = 0; i < array.Count; i++)
                {
                    if (handler(array[i]))
                    {
                        return array[i];
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获得传入元素某个符合条件的所有对象
            /// </summary>
            public static T[] FindAll<T>(IList<T> array, Predicate<T> handler)
            {
                var dstArray = new T[array.Count];
                int idx = 0;
                for (int i = 0; i < array.Count; i++)
                {
                    if (handler(array[i]))
                    {
                        dstArray[idx] = array[i];
                        idx++;
                    }
                }

                Array.Resize(ref dstArray, idx);
                return dstArray;
            }

            /// <summary>
            /// 泛型二分查找，需要传入升序数组
            /// </summary>
            /// <returns>返回对象在数组中的序号，若不存在，则返回-1</returns>
            public static int BinarySearch<T, K>(IList<T> array, K target, Func<T, K> handler)
                where K : IComparable<K>
            {
                int first = 0;
                int last = array.Count - 1;
                while (first <= last)
                {
                    int mid = first + (last - first) / 2;
                    if (handler(array[mid]).CompareTo(target) > 0)
                        last = mid - 1;
                    else if (handler(array[mid]).CompareTo(target) < 0)
                        first = mid + 1;
                    else
                        return mid;
                }

                return -1;
            }

            /// <summary>
            /// 将一个int数组转换为顺序的整数;
            /// 若数组中存在负值，则默认将负值取绝对值
            /// </summary>
            /// <param name="array">传入的数组</param>
            /// <returns>转换成整数后的int</returns>
            public static int ConvertIntArrayToInt(int[] array)
            {
                int result = 0;
                int length = array.Length;
                for (int i = 0; i < length; i++)
                {
                    result += Convert.ToInt32((Math.Abs(array[i]) * Math.Pow(10, length - 1 - i)));
                }

                return result;
            }

            /// <summary>
            /// 生成指定长度的int整数
            /// </summary>
            /// <param name="length">数值长度</param>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int整数</returns>
            public static int RandomRange(int length, int minValue, int maxValue)
            {
                if (minValue >= maxValue)
                    throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
                string buffer = "0123456789"; // 随机字符中也可以为汉字（任何）
                StringBuilder strbuilder = new StringBuilder();
                int range = buffer.Length;
                int resultValue = 0;
                do
                {
                    for (int i = 0; i < length; i++)
                    {
                        strbuilder.Append(buffer.Substring(random.Next(range), 1));
                    }

                    resultValue = Int32.Parse(strbuilder.ToString());
                } while (resultValue > maxValue || resultValue < minValue);

                return resultValue;
            }

            /// <summary>
            /// 随机在范围内生成一个int
            /// </summary>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int整数</returns>
            public static int RandomRange(int minValue, int maxValue)
            {
                if (minValue >= maxValue)
                    throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
                int seed = Guid.NewGuid().GetHashCode();
                Random random = new Random(seed);
                int result = random.Next(minValue, maxValue);
                return result;
            }

            /// <summary>
            /// 随机在范围内生成一个long
            /// </summary>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的long</returns>
            public static long RandomRange(long minValue, long maxValue)
            {
                if (minValue >= maxValue)
                    throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);
                // 计算随机值范围
                long range = maxValue - minValue + 1;
                // 将随机值映射到指定范围内
                long result = (long)Math.Floor(longRand / (double)long.MaxValue * range) + minValue;
                return result;
            }
            
            /// <summary>
            /// 返回一个0.0~1.0之间的随机数
            /// </summary>
            /// <returns>随机数</returns>
            public static double RandomDouble()
            {
                return random.NextDouble();
            }

            /// <summary>
            /// 交换两个值
            /// </summary>
            /// <typeparam name="T">传入的对象类型</typeparam>
            /// <param name="lhs">第一个需要交换的值</param>
            /// <param name="rhs">第二个需要交换的值</param>
            public static void Swap<T>(ref T lhs, ref T rhs)
            {
                (lhs, rhs) = (rhs, lhs);
            }

            /// <summary>
            /// 交换数组中的两个元素
            /// </summary>
            /// <typeparam name="T">传入的对象类型</typeparam>
            /// <param name="array">传入的数组</param>
            /// <param name="lhs">序号A</param>
            /// <param name="rhs">序号B</param>
            public static void Swap<T>(IList<T> array, int lhs, int rhs)
            {
                (array[lhs], array[rhs]) = (array[rhs], array[lhs]);
            }

            /// <summary>
            /// 随机打乱数组
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <param name="array">数组</param>
            public static void Disrupt<T>(IList<T> array)
            {
                Disrupt(array, 0, array.Count);
            }

            /// <summary>
            /// 随机打乱数组
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <param name="array">数组</param>
            /// <param name="startIndex">起始序号</param>
            /// <param name="count">数量</param>
            public static void Disrupt<T>(IList<T> array, int startIndex, int count)
            {
                int index = 0;
                T tmp;
                var endIndex = startIndex + count;
                for (int i = startIndex; i < endIndex; i++)
                {
                    index = RandomRange(startIndex, endIndex);
                    if (index != i)
                    {
                        tmp = array[i];
                        array[i] = array[index];
                        array[index] = tmp;
                    }
                }
            }

            /// <summary>
            /// 产生均匀随机数
            /// </summary>
            public static double AverageRandom(double minValue, double maxValue)
            {
                int min = (int)(minValue * 10000);
                int max = (int)(maxValue * 10000);
                int result = random.Next(min, max);
                return result / 10000.0;
            }

            /// <summary>
            /// 正态分布概率密度函数
            /// </summary>
            public static double NormalDistributionProbability(double x, double miu, double sigma)
            {
                return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) *
                       Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
            }

            /// <summary>
            /// 随机正态分布；
            /// </summary>
            public static double RandomNormalDistribution(double miu, double sigma, double min, double max) //产生正态分布随机数
            {
                double x;
                double dScope;
                double y;
                do
                {
                    x = AverageRandom(min, max);
                    y = NormalDistributionProbability(x, miu, sigma);
                    dScope = AverageRandom(0, NormalDistributionProbability(miu, miu, sigma));
                } while (dScope > y);

                return x;
            }

            /// <summary>
            /// 1或-1的随机值
            /// </summary>
            /// <returns> 1或-1</returns>
            public static int OneOrMinusOne()
            {
                return random.Next(0, 2) * 2 - 1;
            }

            /// <summary>
            /// 数组去重；
            /// </summary>
            /// <typeparam name="T">可比数据类型</typeparam>
            /// <param name="array">源数据</param>
            /// <returns>去重后的数据</returns>
            public static T[] Distinct<T>(IList<T> array)
                where T : IComparable
            {
                var length = array.Count;
                T[] dst = new T[length];
                int idx = 0;
                for (int i = 0; i < length; i++)
                {
                    bool isDuplicate = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (array[i].CompareTo(array[j]) == 0)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (!isDuplicate)
                    {
                        dst[idx] = array[i];
                        idx++;
                    }
                }

                Array.Resize(ref dst, idx);
                return dst;
            }

            /// <summary>
            /// 产生正态分布的随机数
            /// </summary>
            /// <param name="mean">均值</param>
            /// <param name="stdDev">方差</param>
            /// <returns>随机数</returns>
            public static double NextGauss(double mean, double stdDev)
            {
                double u1 = 1.0 - random.NextDouble();
                double u2 = 1.0 - random.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                return mean + stdDev * randStdNormal;
            }

            /// <summary>
            /// Fisher–Yates shuffle 洗牌算法
            /// </summary>
            public static void Shuffle<T>(IList<T> array, int randomValue)
            {
                var random = new Random(randomValue);
                for (int i = array.Count - 1; i > 0; i--)
                {
                    int randomIndex = random.Next(0, i + 1);

                    // 交换元素位置
                    (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
                }
            }
            
            /// <summary>
            /// 是否是奇数
            /// </summary>
            /// <param name="value">检测的值</param>
            /// <returns>是否是奇数</returns>
            public static bool IsOdd(long value)
            {
                return !Convert.ToBoolean(value & 0x1);
            }
            
            /// <summary>
            /// 是否是偶数
            /// </summary>
            /// <param name="value">检测的值</param>
            /// <returns>是否是偶数</returns>
            public static bool IsEven(long value)
            {
                return Convert.ToBoolean(value & 0x1);
            }
        }
    }
}