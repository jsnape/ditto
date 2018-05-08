#region Copyright (c) all rights reserved.
// <copyright file="CsvDataReader.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Ditto.Core;
    using Ditto.DataLoad.Properties;
    using CsvHelper;
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// Csv IDataReader implementation.
    /// </summary>
    [CLSCompliant(false)]
    public class CsvDataReader : IDataReader
    {
        /// <summary>
        /// The fixed fields.
        /// </summary>
        private readonly List<Tuple<string, Func<object>>> fixedFields = new List<Tuple<string, Func<object>>>();

        /// <summary>
        /// The underlying reader.
        /// </summary>
        private readonly CsvReader reader;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly Csv.Configuration configuration;

        /// <summary>
        /// The source file.
        /// </summary>
        private readonly string sourceFile;

        /// <summary>
        /// The normalized headers.
        /// </summary>
        private readonly string[] normalizedHeaders;

        /// <summary>
        /// Current reader state.
        /// </summary>
        private bool closed;

        /// <summary>
        /// The current row.
        /// </summary>
        /// <remarks>Initially -1 to account for the proactive initial read.</remarks>
        private int currentRow = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataReader" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourceFile">The source file name to be embedded in the data stream.</param>
        /// <param name="config">The CSV configuration.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public CsvDataReader(TextReader source, string sourceFile, Csv.Configuration config)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            DittoEventSource.Log.LoadingFile(sourceFile);
            this.sourceFile = sourceFile;
            this.configuration = config ?? throw new ArgumentNullException("config");

            this.reader = new CsvReader(source, config);

            var lastWriteTime = File.GetLastWriteTimeUtc(this.sourceFile);

            // NB: I have made a choice here to prefix internal columns with underscores because
            // I have no idea what the real column names could be. The _prefix is consistent with
            // the old C++ compiler implementation style.
            this.fixedFields.Add(Tuple.Create<string, Func<object>>("_LineNumber", () => this.currentRow + 1));
            this.fixedFields.Add(Tuple.Create<string, Func<object>>("_SourceFile", () => this.sourceFile));
            this.fixedFields.Add(Tuple.Create<string, Func<object>>("_LastWriteTimeUtc", () => lastWriteTime));

            // Many reader functions throw exceptions until the first record has been read.
            this.closed = !this.reader.Read();

            if (this.configuration.HasHeaderRecord)
            {
                this.normalizedHeaders =
                    this.reader.Context.HeaderRecord
                    .Select(h => this.NormalizeFieldHeader(h))
                    .ToArray();
            }
            else
            {
                this.normalizedHeaders =
                    Enumerable.Range(1, this.reader.Context.Record.Length)
                    .Select(x => "Column" + x.ToString("G", CultureInfo.CurrentCulture))
                    .ToArray();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public bool IsClosed
        {
            get { return this.closed; }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get { return this.fixedFields.Count + this.reader.Context.Record.Length; }
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        public int RecordsAffected
        {
            get { return -1; }
        }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>The value of the specified field.</returns>
        public object this[string name]
        {
            get { return this.GetValue(this.GetOrdinal(name)); }
        }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <param name="i">The index of the value to retrieve.</param>
        /// <returns>The value of the specified field.</returns>
        public object this[int i]
        {
            get { return this.GetValue(i); }
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        public bool Read()
        {
            // Many reader functions throw exceptions until a the first record has been read.
            // Since we auto read the first row in the constructor this check just skips the
            // read call for the subsequent one.
            if (this.currentRow == -1 || this.reader.Read())
            {
                this.currentRow++;
                return true;
            }
            else
            {
                this.closed = true;
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        public decimal GetDecimal(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetDecimal"));
            }

            return this.reader.GetField<decimal>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        public string GetName(int i)
        {
            var allFieldsCount = this.normalizedHeaders.Length + this.fixedFields.Count;

            if (i >= allFieldsCount)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.CsvDataReaderIndexOutOfRangeMessageFormat,
                    nameof(this.GetName),
                    allFieldsCount,
                    this.sourceFile);

                throw new InvalidOperationException(message);
            }

            var name = i < this.fixedFields.Count ? this.fixedFields[i].Item1 : this.normalizedHeaders[i - this.fixedFields.Count];

            Debug.WriteLine("CsvDataReader.GetName({0}) = {1}", i, name);

            return name;
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>
        /// The index of the named field.
        /// </returns>
        public int GetOrdinal(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            int ordinal = 0;

            if (name.StartsWith("_", StringComparison.OrdinalIgnoreCase))
            {
                ordinal = this.fixedFields.FindIndex(x => x.Item1 == name);
            }
            else
            {
                int findIndex = Array.FindIndex(this.normalizedHeaders, x => x == name);
                ordinal = findIndex == -1 ? findIndex : findIndex + this.fixedFields.Count;
            }

            Debug.WriteLine("CsvDataReader.GetOrdinal({0}) = {1}", name, ordinal);

            return ordinal;
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Object" /> which will contain the field value upon return.
        /// </returns>
        public object GetValue(int i)
        {
            // This helps with wierdly formatted data files.
            if (i >= this.fixedFields.Count + this.reader.Context.Record.Length)
            {
                return null;
            }

            if (i < this.fixedFields.Count)
            {
                return this.fixedFields[i].Item2();
            }
            else
            {
                return this.reader.GetField(i - this.fixedFields.Count);
            }
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object" /> to copy the attribute fields into.</param>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object" /> in the array.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// True if the specified field is set to null; otherwise, false.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using defined interface.")]
        public bool IsDBNull(int i)
        {
            bool isDBNull = string.IsNullOrEmpty(this.GetValue(i).ToString());
            return isDBNull;
        }

        #region IDataReader forwarding functions

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader" /> Object.
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The value of the column.
        /// </returns>
        public bool GetBoolean(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetBoolean"));
            }

            return this.reader.GetField<bool>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        public byte GetByte(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetByte"));
            }

            return this.reader.GetField<byte>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        public char GetChar(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetChar"));
            }

            return this.reader.GetField<char>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        public DateTime GetDateTime(int i)
        {
            if (i < this.fixedFields.Count)
            {
                return (DateTime)this.fixedFields[i].Item2();
            }
            else
            {
                return this.reader.GetField<DateTime>(i - this.fixedFields.Count);
            }
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        public double GetDouble(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetDouble"));
            }

            return this.reader.GetField<double>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        public float GetFloat(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetFloat"));
            }

            return this.reader.GetField<float>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The GUID value of the specified field.
        /// </returns>
        public Guid GetGuid(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetGuid"));
            }

            return this.reader.GetField<Guid>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        public short GetInt16(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetInt16"));
            }

            return this.reader.GetField<short>(i - this.fixedFields.Count);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        public int GetInt32(int i)
        {
            if (i < this.fixedFields.Count)
            {
                return (int)this.fixedFields[i].Item2();
            }
            else
            {
                return this.reader.GetField<int>(i - this.fixedFields.Count);
            }
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        public long GetInt64(int i)
        {
            if (i < this.fixedFields.Count)
            {
                throw new InvalidOperationException(this.FormatInvalidIndexExceptionMessage("GetInt64"));
            }
            else
            {
                return this.reader.GetField<long>(i - this.fixedFields.Count);
            }
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The string value of the specified field.
        /// </returns>
        public string GetString(int i)
        {
            if (i < this.fixedFields.Count)
            {
                return (string)this.fixedFields[i].Item2();
            }
            else
            {
                return this.reader.GetField<string>(i - this.fixedFields.Count);
            }
        }

        #endregion

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        public bool NextResult()
        {
            return false;
        }

        #region IDataReader not implemented

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>
        /// The actual number of bytes read.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>
        /// The actual number of characters read.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented.</exception>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }

        /// <summary>
        /// Formats the invalid index exception message.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>A formatted exception message.</returns>
        private string FormatInvalidIndexExceptionMessage(string functionName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.CsvDataReaderInvalidIndexMessageFormat,
                functionName,
                this.fixedFields.Count);
        }

        /// <summary>
        /// Normalizes the field header.
        /// </summary>
        /// <remarks>
        /// This function exists because the CsvHelper/CsvReader class doesn't
        /// look at the config values when loading the header row.
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <returns>A normalized header.</returns>
        private string NormalizeFieldHeader(string value)
        {
            return this.reader.Configuration.PrepareHeaderForMatch(value);
        }
    }
}
