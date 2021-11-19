﻿using Payment.Zvt.Models;
using System;

namespace Payment.Zvt.Helpers
{
    /// <summary>
    /// Apdu Helper
    /// </summary>
    public static class ApduHelper
    {
        private const int ControlFieldLength = 2;
        private const byte ExtendedLengthFieldIndicator = 0xFF;
        private const byte ExtendedLengthFieldByteCount = 2;

        /// <summary>
        /// Get Apdu Info
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApduResponseInfo GetApduInfo(byte[] data)
        {
            if (data == null || data.Length < 3)
            {
                // More than 2 bytes required
                //
                // 00-00-00
                // |  |  |  
                // │  │  └─ Length
                // │  └─ Control field INSTR
                // └─ Control field CLASS

                return new ApduResponseInfo();
            }

            var apduDefaultLengthByteCount = 1;

            var item = new ApduResponseInfo();
            item.ControlField = data.Slice(0, ControlFieldLength).ToArray();

            var packageData = data.Slice(ControlFieldLength, 1);
            var startIndex = ControlFieldLength + apduDefaultLengthByteCount;

            if (packageData[0] != ExtendedLengthFieldIndicator)
            {
                item.DataLength = packageData[0];
                item.DataStartIndex = startIndex;
            }
            else
            {
                item.DataLength = BitConverter.ToInt16(data.Slice(startIndex, ExtendedLengthFieldByteCount), 0);
                item.DataStartIndex = startIndex + ExtendedLengthFieldByteCount;
            }

            return item;
        }
    }
}
