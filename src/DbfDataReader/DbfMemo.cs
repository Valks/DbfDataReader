﻿using System;
using System.IO;
using System.Text;

namespace DbfDataReader
{
    public abstract class DbfMemo : Disposable
    {
        protected const int BlockHeaderSize = 8;
        protected const int DefaultBlockSize = 512;

        protected BinaryReader _binaryReader;

        protected DbfMemo(string path)
            : this(path, Encoding.UTF8)
        {
        }

        protected DbfMemo(string path, Encoding encoding)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            Path = path;
            CurrentEncoding = encoding;

            var stream = new FileStream(path, FileMode.Open);
            _binaryReader = new BinaryReader(stream, encoding);
        }

        protected DbfMemo(Stream stream, Encoding encoding)
        {
            Path = string.Empty;
            CurrentEncoding = encoding;

            _binaryReader = new BinaryReader(stream, encoding);
        }

        public Encoding CurrentEncoding { get; set; }

        public void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing) return;
                _binaryReader?.Dispose();
            }
            finally
            {
                _binaryReader = null;
            }
        }

        public virtual int BlockSize => DefaultBlockSize;

        public string Path { get; set; }

        public abstract string BuildMemo(long startBlock);

        public string Get(long startBlock)
        {
            return startBlock <= 0 ? string.Empty : BuildMemo(startBlock);
        }

        public long Offset(long startBlock)
        {
            return startBlock * BlockSize;
        }

        public int ContentSize(int memoSize)
        {
            return (memoSize - BlockSize) + BlockHeaderSize;
        }

        public int BlockContentSize()
        {
            return BlockSize + BlockHeaderSize;
        }
    }
}