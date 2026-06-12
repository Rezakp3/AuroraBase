using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.ServiceFeatures.Models;

public class ServiceDto
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string ServiceIdentifier { get; set; }
}
