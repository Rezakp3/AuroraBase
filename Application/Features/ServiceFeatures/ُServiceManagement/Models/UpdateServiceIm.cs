using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.Models;

public class UpdateServiceIm : AddServiceIm
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
}