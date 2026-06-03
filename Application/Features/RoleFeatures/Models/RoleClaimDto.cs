using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.Models;

public class RoleClaimDto
{
    [RequiredFa, DisplayName("تایپ"), MaxLengthFa(10)]
    public string Type { get; set; }
    [RequiredFa, DisplayName("مقدار"), MaxLengthFa(30)]
    public string Value { get; set; }
}
