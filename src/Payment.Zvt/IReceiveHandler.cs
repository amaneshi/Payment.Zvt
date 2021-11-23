﻿using Payment.Zvt.Models;
using System;

namespace Payment.Zvt
{
    /// <summary>
    /// ReceiveHandler Interface
    /// </summary>
    public interface IReceiveHandler
    {
        /// <summary>
        /// Command Completion received
        /// </summary>
        event Action CompletionReceived;

        /// <summary>
        /// Command Abort received
        /// </summary>
        event Action<string> AbortReceived;

        /// <summary>
        /// Command NotSupported received
        /// </summary>
        event Action NotSupportedReceived;

        /// <summary>
        /// StatusInformation received
        /// </summary>
        event Action<StatusInformation> StatusInformationReceived;

        /// <summary>
        /// IntermediateStatusInformation received
        /// </summary>
        event Action<string> IntermediateStatusInformationReceived;

        /// <summary>
        /// Line received
        /// </summary>
        event Action<PrintLineInfo> LineReceived;

        /// <summary>
        /// Receipt received
        /// </summary>
        event Action<ReceiptInfo> ReceiptReceived;

        /// <summary>
        /// Process received data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool ProcessData(Span<byte> data);
    }
}