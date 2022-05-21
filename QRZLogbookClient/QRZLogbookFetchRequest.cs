using System;
using ADIF.NET;

namespace QRZLogbookClient {

  public class QRZLogbookFetchRequest : QRZLogbookRequest {

    public bool ConfirmedOnly { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? DXCC { get; set; }

    public string Band { get; set; }

    public string Mode { get; set; }

    public string Call { get; set; }

    public string[] LogIDs { get; set; }

    public int? MaxRecords { get; set; }

    public bool LogIDsOnly { get; set; }

    public QRZLogbookFetchRequest(string key) : base(key, QRZAction.Fetch) { }

    public override IQRZResponse Execute()
    {
      if (StartDate.HasValue && !EndDate.HasValue || EndDate.HasValue && !StartDate.HasValue)
        throw new InvalidOperationException("Start and end dates are required.");

      if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
        throw new InvalidOperationException("Start date must be before end date.");

      if (MaxRecords.HasValue && MaxRecords.Value < 0)
        throw new InvalidOperationException("Max records cannot be less than zero.");

      if (DXCC.HasValue && DXCC.Value < 1)
        throw new InvalidOperationException("DXCC must be greater than zero.");

      if (DXCC.HasValue)
        SetOption($"{QRZOption.DXCC}:{DXCC.Value}");

      if (StartDate.HasValue && EndDate.HasValue)
        SetOption($"{QRZOption.Between}:{StartDate.Value.ToString("yyyy-MM-dd")}+{EndDate.Value.ToString("yyyy-MM-dd")}");

      if (!string.IsNullOrEmpty(Band))
        SetOption($"{QRZOption.Band}:{Band}");

      if (!string.IsNullOrEmpty(Mode))
        SetOption($"{QRZOption.Mode}:{Mode}");

      if (!string.IsNullOrEmpty(Call))
        SetOption($"{QRZOption.Call}:{Call}");

      if (LogIDs != null && LogIDs.Length > 0)
        SetOption($"{QRZOption.LogIDs}:{string.Join(",", LogIDs)}");

      if (MaxRecords.HasValue)
        SetOption($"{QRZOption.Max}:{MaxRecords.Value}");

      if (string.IsNullOrEmpty(Option))
        SetOption(QRZOption.All);

      if (LogIDsOnly)
        SetOption($"{QRZOption.Type}:LOGIDS");

      if (ConfirmedOnly)
        SetOption($"{QRZOption.Status}:CONFIRMED");
      else
        SetOption($"{QRZOption.Status}:ALL");

      var response = GetResponse(GetRequest());
      return new QRZFetchResponse(GetResponseCollection(response));
    }
  }
}
