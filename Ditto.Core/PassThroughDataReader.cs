#region Copyright (c) all rights reserved.
// <copyright file="PassThroughDataReader.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion Copyright (c) all rights reserved.

namespace Ditto.Core
{
    using System;
    using System.Data;

    /// <summary>
    /// Pass Through Data Reader.
    /// </summary>
    /// <remarks>This is a base class to save re-implementing a lot of the pass through functions.</remarks>
    public abstract class PassThroughDataReader : IDataReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassThroughDataReader" /> class.
        /// </summary>
        protected PassThroughDataReader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PassThroughDataReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected PassThroughDataReader(IDataReader reader)
        {
            this.Reader = reader ?? throw new ArgumentNullException("reader");
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        public virtual int Depth
        {
            get { return this.Reader.Depth; }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public virtual bool IsClosed
        {
            get { return this.Reader.IsClosed; }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        public virtual int RecordsAffected
        {
            get { return this.Reader.RecordsAffected; }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public virtual int FieldCount
        {
            get { return this.Reader.FieldCount; }
        }

        /// <summary>
        /// Gets or sets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        protected IDataReader Reader { get; set; }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The value of this column.</returns>
        public virtual object this[string name]
        {
            get { return this.Reader[name]; }
        }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The value of this column.</returns>
        public virtual object this[int i]
        {
            get { return this.Reader[i]; }
        }

        /// <summary>
        /// Closes the <see cref="System.Data.IDataReader" /> Object.
        /// </summary>
        public virtual void Close() => this.Dispose();

        /// <summary>
        /// Returns a <see cref="System.Data.DataTable" /> that describes the column metadata of the <see cref="System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        public virtual DataTable GetSchemaTable()
        {
            return this.Reader.GetSchemaTable();
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        public virtual bool NextResult()
        {
            return this.Reader.NextResult();
        }

        /// <summary>
        /// Advances the <see cref="System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        public virtual bool Read()
        {
            return this.Reader.Read();
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The value of the column.
        /// </returns>
        public virtual bool GetBoolean(int i)
        {
            return this.Reader.GetBoolean(i);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        public virtual byte GetByte(int i)
        {
            return this.Reader.GetByte(i);
        }

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
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this.Reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        public virtual char GetChar(int i)
        {
            return this.Reader.GetChar(i);
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
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this.Reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Returns an <see cref="System.Data.IDataReader" /> for the specified column ordinal.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="System.Data.IDataReader" /> for the specified column ordinal.
        /// </returns>
        public virtual IDataReader GetData(int i)
        {
            return this.Reader.GetData(i);
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        public virtual string GetDataTypeName(int i)
        {
            return this.Reader.GetDataTypeName(i);
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        public virtual DateTime GetDateTime(int i)
        {
            return this.Reader.GetDateTime(i);
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        public virtual decimal GetDecimal(int i)
        {
            return this.Reader.GetDecimal(i);
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        public virtual double GetDouble(int i)
        {
            return this.Reader.GetDouble(i);
        }

        /// <summary>
        /// Gets the <see cref="Type" /> information corresponding to the type of <see cref="object" />
        /// that would be returned from <see cref="System.Data.IDataRecord.GetValue(Int32)" />.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="Type" /> information corresponding to the type of <see cref="object" />
        /// that would be returned from <see cref="System.Data.IDataRecord.GetValue(Int32)" />.
        /// </returns>
        public virtual Type GetFieldType(int i)
        {
            return this.Reader.GetFieldType(i);
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        public virtual float GetFloat(int i)
        {
            return this.Reader.GetFloat(i);
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The GUID value of the specified field.
        /// </returns>
        public virtual Guid GetGuid(int i)
        {
            return this.Reader.GetGuid(i);
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        public virtual short GetInt16(int i)
        {
            return this.Reader.GetInt16(i);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        public virtual int GetInt32(int i)
        {
            return this.Reader.GetInt32(i);
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        public virtual long GetInt64(int i)
        {
            return this.Reader.GetInt64(i);
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        public virtual string GetName(int i)
        {
            return this.Reader.GetName(i);
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>
        /// The index of the named field.
        /// </returns>
        public virtual int GetOrdinal(string name)
        {
            return this.Reader.GetOrdinal(name);
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The string value of the specified field.
        /// </returns>
        public virtual string GetString(int i)
        {
            return this.Reader.GetString(i);
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="System.Object" /> which will contain the field value upon return.
        /// </returns>
        public virtual object GetValue(int i)
        {
            return this.Reader.GetValue(i);
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="System.Object" /> to copy the attribute fields into.</param>
        /// <returns>
        /// The number of instances of <see cref="System.Object" /> in the array.
        /// </returns>
        public virtual int GetValues(object[] values)
        {
            return this.Reader.GetValues(values);
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// True if the specified field is set to null; otherwise, false.
        /// </returns>
        public virtual bool IsDBNull(int i)
        {
            return this.Reader.IsDBNull(i);
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
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Reader != null)
            {
                this.Reader.Dispose();
            }
        }
    }
}
