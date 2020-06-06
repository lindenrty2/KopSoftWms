using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace Services.Outside
{
    public class SelfReservoirAreaManager
    {
        private static int _count1 = 0;
        private static int _count2 = 0;

        public static string GetPLC(long reservoirAreaId, PLCPosition pos)
        {
            if (reservoirAreaId == 1)
            {
                if (pos == PLCPosition.Left)
                {
                    return "1010";
                }
                else if (pos == PLCPosition.Right)
                {
                    return "1020";
                }
                else
                {
                    _count1++;
                    return _count1 % 2 == 1 ? "1010" : "1020";
                }
            }
            else
            {
                if (pos == PLCPosition.Left)
                {
                    return "5010";
                }
                else if (pos == PLCPosition.Right)
                {
                    return "6010";
                }
                else
                {
                    _count2++;
                    return _count2 % 2 == 1 ? "5010" : "6010";
                }
            }
        }

        /// <summary>
        /// 是否是固定料箱位置的库区
        /// </summary>
        /// <returns></returns>
        public static bool IsPositionFix(long reservoirAreaId)
        {
            return reservoirAreaId == 1 || reservoirAreaId == 3;
        }
    }
}
