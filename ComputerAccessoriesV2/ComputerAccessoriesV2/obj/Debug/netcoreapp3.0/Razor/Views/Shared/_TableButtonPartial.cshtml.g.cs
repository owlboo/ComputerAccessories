#pragma checksum "D:\HK7\git\ComputerAccessories\ComputerAccessoriesV2\ComputerAccessoriesV2\Views\Shared\_TableButtonPartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0449ef40929d57b34f73c0e38c0c0ce059347739"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__TableButtonPartial), @"mvc.1.0.view", @"/Views/Shared/_TableButtonPartial.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\HK7\git\ComputerAccessories\ComputerAccessoriesV2\ComputerAccessoriesV2\Views\_ViewImports.cshtml"
using ComputerAccessoriesV2;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\HK7\git\ComputerAccessories\ComputerAccessoriesV2\ComputerAccessoriesV2\Views\_ViewImports.cshtml"
using ComputerAccessoriesV2.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0449ef40929d57b34f73c0e38c0c0ce059347739", @"/Views/Shared/_TableButtonPartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b9b6ae8cb1b669db54b99bf3851d783e547ddf64", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__TableButtonPartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<int>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\HK7\git\ComputerAccessories\ComputerAccessoriesV2\ComputerAccessoriesV2\Views\Shared\_TableButtonPartial.cshtml"
       
         string model = Model.ToString();
         string edit = @"Edit" + @ViewBag.controller+@"/"+model;
         string delete = @"Delete" + @ViewBag.controller+@"/"+model;


      

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        <div class=\"btn-group\" role=\"group\" style=\"height:44px; \">\r\n            <a type=\"button\" class=\"btn btn-success\"");
            BeginWriteAttribute("href", " href=\"", 339, "\"", 383, 1);
#nullable restore
#line 13 "D:\HK7\git\ComputerAccessories\ComputerAccessoriesV2\ComputerAccessoriesV2\Views\Shared\_TableButtonPartial.cshtml"
WriteAttributeValue("", 346, Url.Action(edit).Replace("%2F",@"/"), 346, 37, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("><i class=\"fa fa-edit\"></i></a>\r\n        </div>\r\n  ");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<int> Html { get; private set; }
    }
}
#pragma warning restore 1591