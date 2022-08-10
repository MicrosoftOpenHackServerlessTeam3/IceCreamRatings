using System;

namespace IceCreamRatings.Entities;

public class Rate
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string ProductId { get; set; }
    public string Timestamp { get; set; }
    public string LocationName { get; set; }
    public int Rating { get; set; }
    public string UserNotes { get; set; }
}