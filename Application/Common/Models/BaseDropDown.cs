using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Models;

public class BaseDropDown<T>
{
    public T Id { get; set; }
    public string Value { get; set; }
    public bool IsSelected { get; set; }
}
