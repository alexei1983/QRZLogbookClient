using System;
using System.Collections.Generic;
using ADIF.NET;

namespace QRZLogbookClient {

  /// <summary>
  /// 
  /// </summary>
  public class QRZLogbookInsertRequest : QRZLogbookRequest {

    /// <summary>
    /// 
    /// </summary>
    public ADIFQSO QSO { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Replace { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public QRZLogbookInsertRequest(string key) : this(key, null, false) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="qso"></param>
    /// <param name="replace"></param>
    public QRZLogbookInsertRequest(string key, ADIFQSO qso, bool replace = false) : base(key, QRZAction.Insert)
    {
      Replace = replace;
      QSO = qso;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override IQRZResponse Execute()
    {
      if (QSO == null)
        throw new InvalidOperationException("ADIF QSO is required.");

      if (Replace)
        SetOption(QRZOption.Replace);

      var response = GetResponse(GetRequest(new Dictionary<string, string>() { { QRZFields.ADIF, QSO.ToString("A") } }));
      return new QRZInsertResponse(GetResponseCollection(response));
    }
  }
}
