﻿using Microsoft.Extensions.Logging;
using Portalum.Payment.Zvt.Models;
using Portalum.Payment.Zvt.Repositories;
using Portalum.Payment.Zvt.Responses;
using System;
using System.Text;

namespace Portalum.Payment.Zvt.Parsers
{
    /// <summary>
    /// PrintTextBlockParser
    /// </summary>
    public class PrintTextBlockParser : IPrintTextBlockParser
    {
        private readonly ILogger _logger;
        private readonly BmpParser _bmpParser;
        private readonly TlvParser _tlvParser;

        private bool _completelyProcessed;
        private ReceiptType _receiptType;
        private readonly StringBuilder _receiptContent;

        /// <summary>
        /// PrintTextBlockParser
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="errorMessageRepository"></param>
        public PrintTextBlockParser(
            ILogger logger,
            IErrorMessageRepository errorMessageRepository)
        {
            this._logger = logger;

            var tlvInfos = new TlvInfo[]
            {
                new TlvInfo { Tag = "1F07", Description = "ReceiptType", TryProcess = this.SetReceiptType },
                new TlvInfo { Tag = "25", Description = "Print-Texts", TryProcess = this.CleanupReceiptBuffer },
                new TlvInfo { Tag = "07", Description = "Text-Lines", TryProcess = this.AddTextLine },
                new TlvInfo { Tag = "09", Description = "EndOfReceipt", TryProcess = this.EndOfReceipt }
            };

            this._tlvParser = new TlvParser(logger, tlvInfos);
            this._bmpParser = new BmpParser(logger, errorMessageRepository, this._tlvParser);

            this._receiptType = ReceiptType.Unknown;
            this._receiptContent = new StringBuilder();
        }

        /// <inheritdoc />
        public ReceiptInfo Parse(byte[] data)
        {
            this._completelyProcessed = false;

            if (!this._bmpParser.Parse(data, null))
            {
                this._logger.LogError($"{nameof(Parse)} - Error on parsing data");
                return null;
            }

            return new ReceiptInfo
            {
                ReceiptType = this._receiptType,
                Content = this._receiptContent.ToString(),
                CompletelyProcessed = this._completelyProcessed
            };
        }

        private bool SetReceiptType(byte[] data, IResponse response)
        {
            if (data == null || data.Length == 0)
            {
                return true;
            }

            this._receiptType = (ReceiptType)data[0];

            return true;
        }

        private bool CleanupReceiptBuffer(byte[] data, IResponse response)
        {
            this._receiptContent.Clear();

            return true;
        }

        private bool AddTextLine(byte[] data, IResponse response)
        {
            //var textBlock1 = Encoding.UTF7.GetString(data);
            var textBlock = Encoding.GetEncoding(437).GetString(data);
            this._receiptContent.AppendLine(textBlock);

            return true;
        }

        private bool EndOfReceipt(byte[] data, IResponse response)
        {
            this._completelyProcessed = true;

            return true;
        }
    }
}
