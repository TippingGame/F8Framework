namespace Excel
{
    using System;
    using System.Data;
    using System.IO;

    public interface IExcelDataReader : IDataReader, IDisposable, IDataRecord
    {
        DataSet AsDataSet();
        DataSet AsDataSet(bool convertOADateTime);
        void Initialize(Stream fileStream);

        bool IsValid { get; }

        string ExceptionMessage { get; }

        string Name { get; }

        string VisibleState { get; }

        int ResultsCount { get; }

        bool IsFirstRowAsColumnNames { get; set; }
    }
}

