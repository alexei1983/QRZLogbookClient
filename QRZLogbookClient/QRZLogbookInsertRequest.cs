using System;
using System.Collections.Generic;
using ADIF.NET;

namespace QRZLogbookClient {

  public class QRZLogbookInsertRequest : QRZLogbookRequest {

    public ADIFQSO QSO { get; set; }

    public bool Replace { get; set; }

    public QRZLogbookInsertRequest(string key) : this(key, null, false) { }

    public QRZLogbookInsertRequest(string key, ADIFQSO qso, bool replace = false) : base(key, QRZAction.Insert)
    {
      Replace = replace;
      QSO = qso;
    }

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
