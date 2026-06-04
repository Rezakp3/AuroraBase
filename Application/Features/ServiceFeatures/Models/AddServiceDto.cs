using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.Models;

public class AddServiceIm
{
    [RequiredFa, DisplayName("نام سرویس"), MaxLengthFa(30)]
    public string ServiceName { get; set; }
    [DisplayName("توضیح"), MaxLengthFa(250)]
    public string Description { get; set; }
    [RequiredFa, DisplayName("آدرس سرویس"), MaxLengthFa(60)]
    public string Address { get; set; }
    [RequiredFa, DisplayName("احراز"), MaxLengthFa(60)]
    public string ServiceIdentifier { get; set; }
}
