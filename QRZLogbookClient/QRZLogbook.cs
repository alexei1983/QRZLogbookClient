using System;
using System.Collections.Generic;
using ADIF.NET;

namespace QRZLogbookClient {

  /// <summary>
  /// 
  /// </summary>
  public class QRZLogbook {

    /// <summary>
    /// 
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public QRZLogbook() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public QRZLogbook(string key)
    {
      Key = key;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logIDs"></param>
    /// <returns></returns>
    public bool Delete(params string[] logIDs)
    {
      if (logIDs == null || logIDs.Length < 1)
        throw new ArgumentException("At least one log ID is required.", nameof(logIDs));

      if (new QRZLogbookDeleteRequest(Key) { LogIDs = logIDs }.Execute() is QRZDeleteResponse result) {

        if (QRZResult.Partial.Equals(result.Result) && result.LogIDs?.Length > 0)
          throw new InvalidOperationException($"QRZ Error: {result.LogIDs.Length} record(s) not found while attempting delete operation.");
        else if (QRZResult.Fail.Equals(result.Result))
          throw new InvalidOperationException($"QRZ Error: {result.Reason ?? string.Empty}");

        return result.Count > 0;
      }

      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qso"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    public string Insert(ADIFQSO qso, bool replace = false)
    {
      if (qso == null)
        throw new ArgumentNullException(nameof(qso), "QSO is required.");

      if (new QRZLogbookInsertRequest(Key, qso, replace).Execute() is QRZInsertResponse result)
      {
        if (QRZResult.Fail.Equals(result.Result))
          throw new InvalidOperationException($"QRZ Error: {result.Reason ?? string.Empty}");

        return result.LogID;
      }

      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IDictionary<string, string> Status()
    {
      if (new QRZLogbookStatusRequest(Key).Execute() is QRZStatusResponse result)
      {
        if (QRZResult.Fail.Equals(result.Result))
          throw new InvalidOperationException($"QRZ Error: {result.Reason ?? string.Empty}");

        return result.Data;
      }

      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logIDs"></param>
    /// <returns></returns>
    public IDictionary<string, string> Status(params string[] logIDs)
    {
      if (logIDs == null || logIDs.Length < 1)
        throw new ArgumentException("At least one log ID is required.", nameof(logIDs));

      if (new QRZLogbookStatusRequest(Key) { LogIDs = logIDs }.Execute() is QRZStatusResponse result)
      {
        if (QRZResult.Fail.Equals(result.Result))
          throw new InvalidOperationException($"QRZ Error: {result.Reason ?? string.Empty}");

        return result.Data;
      }

      return null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="logIDs"></param>
    /// <returns></returns>
    public ADIFQSOCollection Fetch(params string[] logIDs)
    {
      if (logIDs == null || logIDs.Length < 1)
        throw new ArgumentException("At least one log ID is required.", nameof(logIDs));

      return Fetch(new QRZLogbookFetchRequest(Key)
      {
        LogIDs = logIDs
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="band"></param>
    /// <param name="mode"></param>
    /// <param name="confirmedOnly"></param>
    /// <param name="maxRecords"></param>
    /// <returns></returns>
    public ADIFQSOCollection Fetch(string band,
                                   string mode,
                                   bool confirmedOnly = false,
                                   int maxRecords = 0)
    {
      if (string.IsNullOrEmpty(band) && string.IsNullOrEmpty(mode))
        throw new ArgumentException("Band and/or mode is required");

      return Fetch(new QRZLogbookFetchRequest(Key)
      {
        Band = band,
        Mode = mode,
        ConfirmedOnly = confirmedOnly,
        MaxRecords = maxRecords > 0 ? maxRecords : (int?)null
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="confirmedOnly"></param>
    /// <param name="maxRecords"></param>
    /// <returns></returns>
    public ADIFQSOCollection Fetch(DateTime start,
                                   DateTime end,
                                   bool confirmedOnly = false,
                                   int maxRecords = 0)
    {
      if (end < start)
        throw new ArgumentException("Start date must be before end date.");

      return Fetch(new QRZLogbookFetchRequest(Key)
      {
        StartDate = start,
        EndDate = end,
        ConfirmedOnly = confirmedOnly,
        MaxRecords = maxRecords > 0 ? maxRecords : (int?)null
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="call"></param>
    /// <param name="confirmedOnly"></param>
    /// <param name="maxRecords"></param>
    /// <returns></returns>
    public ADIFQSOCollection Fetch(string call,
                                   bool confirmedOnly = false,
                                   int maxRecords = 0)
    {
      if (string.IsNullOrEmpty(call))
        throw new ArgumentException("Callsign is required.", nameof(call));

      return Fetch(new QRZLogbookFetchRequest(Key)
      {
        Call = call,
        ConfirmedOnly = confirmedOnly,
        MaxRecords = maxRecords > 0 ? maxRecords : (int?)null
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dxcc"></param>
    /// <param name="confirmedOnly"></param>
    /// <param name="maxRecords"></param>
    /// <returns></returns>
    public ADIFQSOCollection Fetch(int dxcc,
                                   bool confirmedOnly = false,
                                   int maxRecords = 0)
    {
      if (dxcc < 1)
        throw new ArgumentException("DXCC must be greater than zero.", nameof(dxcc));

      return Fetch(new QRZLogbookFetchRequest(Key)
      {
        DXCC = dxcc,
        ConfirmedOnly = confirmedOnly,
        MaxRecords = maxRecords > 0 ? maxRecords : (int?)null
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    ADIFQSOCollection Fetch(QRZLogbookFetchRequest request)
    {
      if (request.Execute() is QRZFetchResponse result)
      {
        if (QRZResult.Fail.Equals(result.Result))
          throw new InvalidOperationException($"QRZ Error: {result.Reason ?? string.Empty}");

        return result.ADIF;
      }

      return null;
    }
  }
}
