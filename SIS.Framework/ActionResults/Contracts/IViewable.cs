using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.Framework.ActionResults.Contracts
{
    public interface IViewable : IActionResult
    {
        IRenderable View { get; set; }
    }
}
